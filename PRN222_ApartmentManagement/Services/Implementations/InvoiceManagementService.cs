using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class InvoiceManagementService : IInvoiceManagementService
{
    private readonly ApartmentDbContext _context;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IInvoiceDetailRepository _invoiceDetailRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IEmailService _emailService;
    private readonly IActivityLogService _activityLogService;

    public InvoiceManagementService(
        ApartmentDbContext context,
        IInvoiceRepository invoiceRepository,
        IInvoiceDetailRepository invoiceDetailRepository,
        INotificationRepository notificationRepository,
        IEmailService emailService,
        IActivityLogService activityLogService)
    {
        _context = context;
        _invoiceRepository = invoiceRepository;
        _invoiceDetailRepository = invoiceDetailRepository;
        _notificationRepository = notificationRepository;
        _emailService = emailService;
        _activityLogService = activityLogService;
    }

    public async Task<(bool Success, string Message, int CreatedCount)> GenerateInvoicesAsync(
        int billingMonth,
        int billingYear,
        int dueDay,
        int createdByUserId)
    {
        if (billingMonth < 1 || billingMonth > 12)
        {
            return (false, "Tháng lập hóa đơn không hợp lệ.", 0);
        }

        if (billingYear < 2000 || billingYear > 3000)
        {
            return (false, "Năm lập hóa đơn không hợp lệ.", 0);
        }

        var issueDate = new DateTime(billingYear, billingMonth, 1);
        var dueDate = new DateTime(billingYear, billingMonth, Math.Min(dueDay, DateTime.DaysInMonth(billingYear, billingMonth)));
        var monthStart = new DateTime(billingYear, billingMonth, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        var apartments = await _context.Apartments
            .Include(a => a.ApartmentServices)
            .ToListAsync();

        var createdCount = 0;

        foreach (var apartment in apartments)
        {
            if (await _invoiceRepository.ExistsForApartmentMonthAsync(apartment.ApartmentId, billingMonth, billingYear))
            {
                continue;
            }

            // Chỉ tạo hóa đơn khi căn có chủ hộ đang ở
            var hasOwner = await _context.ResidentApartments
                .AnyAsync(ra =>
                    ra.ApartmentId == apartment.ApartmentId &&
                    ra.IsActive &&
                    ra.ResidencyType == ResidencyType.Owner &&
                    ra.Contract != null &&
                    ra.Contract.Status == ContractStatus.Active);

            if (!hasOwner)
            {
                continue;
            }

            var recurringServices = apartment.ApartmentServices
                .Where(s => s.IsActive &&
                            s.RegisteredFrom <= monthEnd &&
                            (!s.RegisteredTo.HasValue || s.RegisteredTo.Value >= monthStart))
                .ToList();

            var serviceOrders = await _context.ServiceOrders
                .Include(so => so.ServiceType)
                .Where(so =>
                    so.ApartmentId == apartment.ApartmentId &&
                    so.Status == ServiceOrderStatus.Completed &&
                    so.InvoiceId == null &&
                    so.CompletedAt.HasValue &&
                    so.CompletedAt.Value.Month == billingMonth &&
                    so.CompletedAt.Value.Year == billingYear)
                .ToListAsync();

            if (!recurringServices.Any() && !serviceOrders.Any())
            {
                continue;
            }

            var invoice = new Invoice
            {
                InvoiceNumber = await _invoiceRepository.GenerateInvoiceNumberAsync(billingMonth, billingYear, apartment.ApartmentId),
                ApartmentId = apartment.ApartmentId,
                BillingMonth = billingMonth,
                BillingYear = billingYear,
                IssueDate = issueDate,
                DueDate = dueDate,
                Status = InvoiceStatus.Unpaid,
                CreatedBy = createdByUserId,
                CreatedAt = DateTime.Now,
                Notes = $"Hóa đơn tự động tháng {billingMonth:D2}/{billingYear}"
            };

            await _invoiceRepository.AddAsync(invoice);

            decimal totalAmount = 0;

            foreach (var apartmentService in recurringServices)
            {
                var servicePrice = await GetEffectiveServicePriceAsync(apartmentService.ServiceTypeId, monthStart, monthEnd);
                if (servicePrice == null) continue;

                var detail = new InvoiceDetail
                {
                    InvoiceId = invoice.InvoiceId,
                    ServiceTypeId = apartmentService.ServiceTypeId,
                    ServicePriceId = servicePrice.ServicePriceId,
                    Quantity = apartmentService.Quantity,
                    UnitPrice = servicePrice.UnitPrice,
                    Description = $"Dich vu dinh ky: {servicePrice.ServiceType.ServiceTypeName}"
                };

                await _invoiceDetailRepository.AddAsync(detail);
                totalAmount += detail.Quantity * detail.UnitPrice;
            }

            foreach (var serviceOrder in serviceOrders)
            {
                var servicePrice = await GetEffectiveServicePriceAsync(serviceOrder.ServiceTypeId, monthStart, monthEnd);
                if (servicePrice == null) continue;

                var unitPrice = serviceOrder.ActualPrice ?? serviceOrder.EstimatedPrice ?? servicePrice.UnitPrice;
                var detail = new InvoiceDetail
                {
                    InvoiceId = invoice.InvoiceId,
                    ServiceTypeId = serviceOrder.ServiceTypeId,
                    ServicePriceId = servicePrice.ServicePriceId,
                    Quantity = 1,
                    UnitPrice = unitPrice,
                    ServiceOrderId = serviceOrder.ServiceOrderId,
                    Description = $"Dich vu phat sinh: {serviceOrder.ServiceType.ServiceTypeName}"
                };

                await _invoiceDetailRepository.AddAsync(detail);
                totalAmount += detail.Quantity * detail.UnitPrice;

                serviceOrder.InvoiceId = invoice.InvoiceId;
                serviceOrder.UpdatedAt = DateTime.Now;
            }

            invoice.TotalAmount = totalAmount;
            invoice.UpdatedAt = DateTime.Now;
            await _invoiceRepository.UpdateAsync(invoice);

            // Tự động phát hành ngay sau khi tạo — chỉ gửi chủ hộ
            await PublishInvoiceToOwnerAsync(invoice);

            createdCount++;
        }

        await _context.SaveChangesAsync();
        return (true, $"Đã tạo {createdCount} hóa đơn và gửi cho chủ hộ.", createdCount);
    }

    public Task<List<Invoice>> GetInvoicesAsync(int? billingMonth, int? billingYear, InvoiceStatus? status)
    {
        return _invoiceRepository.GetFilteredAsync(billingMonth, billingYear, status);
    }

    public async Task<List<Invoice>> GetResidentInvoicesAsync(int residentUserId,
        int? billingMonth, int? billingYear)
    {
        var apartmentIds = await _context.ResidentApartments
            .Where(ra => ra.UserId == residentUserId && ra.IsActive)
            .Select(ra => ra.ApartmentId)
            .ToListAsync();

        if (!apartmentIds.Any()) return new List<Invoice>();

        return await _invoiceRepository.GetByApartmentsAsync(apartmentIds, billingMonth, billingYear);
    }

    public Task<Invoice?> GetInvoiceForManagementAsync(int invoiceId)
    {
        return _invoiceRepository.GetWithDetailsAsync(invoiceId);
    }

    public async Task<Invoice?> GetInvoiceForResidentAsync(int invoiceId, int residentUserId)
    {
        var apartmentIds = await _context.ResidentApartments
            .Where(ra => ra.UserId == residentUserId && ra.IsActive)
            .Select(ra => ra.ApartmentId)
            .ToListAsync();

        if (!apartmentIds.Any()) return null;

        var invoice = await _invoiceRepository.GetWithDetailsAsync(invoiceId);
        if (invoice == null || !apartmentIds.Contains(invoice.ApartmentId))
        {
            return null;
        }

        return invoice;
    }

    public async Task<List<Invoice>> GetResidentPayableInvoicesAsync(int residentUserId,
        int? billingMonth, int? billingYear)
    {
        var apartmentIds = await _context.ResidentApartments
            .Where(ra => ra.UserId == residentUserId && ra.IsActive)
            .Select(ra => ra.ApartmentId)
            .ToListAsync();

        if (!apartmentIds.Any()) return new List<Invoice>();

        var invoices = await _invoiceRepository.GetByApartmentsAsync(apartmentIds, billingMonth, billingYear);

        return invoices
            .Where(i => i.Status == InvoiceStatus.Issued
                        && i.TotalAmount > i.PaidAmount)
            .ToList();
    }

    public async Task<List<PaymentTransaction>> GetResidentTransactionsAsync(int residentUserId,
        int? billingMonth, int? billingYear)
    {
        var apartmentIds = await _context.ResidentApartments
            .Where(ra => ra.UserId == residentUserId && ra.IsActive)
            .Select(ra => ra.ApartmentId)
            .ToListAsync();

        if (!apartmentIds.Any()) return new List<PaymentTransaction>();

        return await _context.PaymentTransactions
            .Include(t => t.Invoice)
            .Where(t => apartmentIds.Contains(t.Invoice.ApartmentId))
            .Where(t => billingMonth == null || t.Invoice.BillingMonth == billingMonth)
            .Where(t => billingYear == null || t.Invoice.BillingYear == billingYear)
            .OrderByDescending(t => t.PaymentDate)
            .ToListAsync();
    }

    /// <summary>
    /// Gửi hóa đơn cho duy nhất chủ hộ (ResidencyType.Owner) của căn.
    /// </summary>
    private async Task PublishInvoiceToOwnerAsync(Invoice invoice)
    {
        var owner = await _context.ResidentApartments
            .Include(ra => ra.User)
            .Where(ra =>
                ra.ApartmentId == invoice.ApartmentId &&
                ra.IsActive &&
                ra.ResidencyType == ResidencyType.Owner &&
                ra.Contract != null &&
                ra.Contract.Status == ContractStatus.Active)
            .Select(ra => ra.User)
            .FirstOrDefaultAsync();

        if (owner == null) return;

        var notification = new Notification
        {
            UserId = owner.UserId,
            Title = $"Hóa đơn tháng {invoice.BillingMonth:D2}/{invoice.BillingYear}",
            Content = $"Hóa đơn {invoice.InvoiceNumber} cần thanh toán {invoice.TotalAmount:N0} VND. Hạn thanh toán: {invoice.DueDate:dd/MM/yyyy}.",
            NotificationType = NotificationType.Invoice,
            ReferenceType = ReferenceType.Invoice,
            ReferenceId = invoice.InvoiceId,
            Priority = NotificationPriority.High,
            CreatedAt = DateTime.Now
        };

        await _notificationRepository.AddAsync(notification);

        if (!string.IsNullOrWhiteSpace(owner.Email))
        {
            try
            {
                await _emailService.SendEmailAsync(
                    owner.Email,
                    $"Hóa đơn {invoice.InvoiceNumber}",
                    $"<p>Xin chào {owner.FullName},</p>" +
                    $"<p>Hóa đơn <strong>{invoice.InvoiceNumber}</strong> đã được phát hành cho căn {invoice.ApartmentId}.</p>" +
                    $"<p><strong>Tổng tiền: {invoice.TotalAmount:N0} VND</strong></p>" +
                    $"<p>Hạn thanh toán: <strong>{invoice.DueDate:dd/MM/yyyy}</strong></p>");
            }
            catch
            {
                // Keep notification flow alive even when email fails.
            }
        }

        invoice.Status = InvoiceStatus.Issued;
        invoice.IsSent = true;
        invoice.SentAt = DateTime.Now;
        invoice.UpdatedAt = DateTime.Now;
        await _invoiceRepository.UpdateAsync(invoice);
        await _activityLogService.LogCustomAsync(
            "PublishInvoice",
            nameof(Invoice),
            invoice.InvoiceId.ToString(),
            $"Phát hành hóa đơn {invoice.InvoiceNumber} cho chủ hộ {owner.FullName}");
    }

    private async Task<ServicePrice?> GetEffectiveServicePriceAsync(int serviceTypeId, DateTime monthStart, DateTime monthEnd)
    {
        return await _context.ServicePrices
            .Include(sp => sp.ServiceType)
            .Where(sp =>
                sp.ServiceTypeId == serviceTypeId &&
                sp.EffectiveFrom <= monthEnd &&
                (!sp.EffectiveTo.HasValue || sp.EffectiveTo.Value >= monthStart))
            .OrderByDescending(sp => sp.EffectiveFrom)
            .FirstOrDefaultAsync();
    }
}
