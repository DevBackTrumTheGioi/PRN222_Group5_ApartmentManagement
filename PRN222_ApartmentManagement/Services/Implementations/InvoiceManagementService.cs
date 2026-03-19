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
                ApprovalStatus = InvoiceApprovalStatus.PendingApproval,
                CreatedBy = createdByUserId,
                CreatedAt = DateTime.Now,
                Notes = $"Hóa đơn tự động tháng {billingMonth:D2}/{billingYear}"
            };

            await _invoiceRepository.AddAsync(invoice);

            decimal totalAmount = 0;

            foreach (var apartmentService in recurringServices)
            {
                var servicePrice = await GetEffectiveServicePriceAsync(apartmentService.ServiceTypeId, monthStart, monthEnd);
                if (servicePrice == null)
                {
                    continue;
                }

                var detail = new InvoiceDetail
                {
                    InvoiceId = invoice.InvoiceId,
                    ServiceTypeId = apartmentService.ServiceTypeId,
                    ServicePriceId = servicePrice.ServicePriceId,
                    Quantity = apartmentService.Quantity,
                    UnitPrice = servicePrice.UnitPrice,
                    Description = $"Dịch vụ định kỳ: {servicePrice.ServiceType.ServiceTypeName}"
                };

                await _invoiceDetailRepository.AddAsync(detail);
                totalAmount += detail.Quantity * detail.UnitPrice;
            }

            foreach (var serviceOrder in serviceOrders)
            {
                var servicePrice = await GetEffectiveServicePriceAsync(serviceOrder.ServiceTypeId, monthStart, monthEnd);
                if (servicePrice == null)
                {
                    continue;
                }

                var unitPrice = serviceOrder.ActualPrice ?? serviceOrder.EstimatedPrice ?? servicePrice.UnitPrice;
                var detail = new InvoiceDetail
                {
                    InvoiceId = invoice.InvoiceId,
                    ServiceTypeId = serviceOrder.ServiceTypeId,
                    ServicePriceId = servicePrice.ServicePriceId,
                    Quantity = 1,
                    UnitPrice = unitPrice,
                    ServiceOrderId = serviceOrder.ServiceOrderId,
                    Description = $"Dịch vụ phát sinh: {serviceOrder.ServiceType.ServiceTypeName}"
                };

                await _invoiceDetailRepository.AddAsync(detail);
                totalAmount += detail.Quantity * detail.UnitPrice;

                serviceOrder.InvoiceId = invoice.InvoiceId;
                serviceOrder.UpdatedAt = DateTime.Now;
            }

            invoice.TotalAmount = totalAmount;
            invoice.UpdatedAt = DateTime.Now;
            await _invoiceRepository.UpdateAsync(invoice);

            createdCount++;
        }

        await _context.SaveChangesAsync();
        return (true, $"Đã tạo {createdCount} hóa đơn tự động.", createdCount);
    }

    public Task<List<Invoice>> GetInvoicesAsync(int? billingMonth, int? billingYear, InvoiceStatus? status, InvoiceApprovalStatus? approvalStatus)
    {
        return _invoiceRepository.GetFilteredAsync(billingMonth, billingYear, status, approvalStatus);
    }

    public async Task<List<Invoice>> GetResidentInvoicesAsync(int residentUserId, int? billingMonth, int? billingYear)
    {
        var resident = await _context.Users.FirstOrDefaultAsync(u => u.UserId == residentUserId && u.Role == UserRole.Resident && !u.IsDeleted);
        if (resident?.ApartmentId == null)
        {
            return new List<Invoice>();
        }

        return await _invoiceRepository.GetByApartmentAsync(resident.ApartmentId.Value, billingMonth, billingYear);
    }

    public Task<Invoice?> GetInvoiceForManagementAsync(int invoiceId)
    {
        return _invoiceRepository.GetWithDetailsAsync(invoiceId);
    }

    public async Task<Invoice?> GetInvoiceForResidentAsync(int invoiceId, int residentUserId)
    {
        var resident = await _context.Users.FirstOrDefaultAsync(u => u.UserId == residentUserId && u.Role == UserRole.Resident && !u.IsDeleted);
        if (resident?.ApartmentId == null)
        {
            return null;
        }

        var invoice = await _invoiceRepository.GetWithDetailsAsync(invoiceId);
        if (invoice == null || invoice.ApartmentId != resident.ApartmentId.Value)
        {
            return null;
        }

        return invoice;
    }

    public async Task<(bool Success, string Message)> ApproveInvoiceAsync(int invoiceId, int approverUserId)
    {
        var approver = await _context.Users.FirstOrDefaultAsync(u => u.UserId == approverUserId && u.Role == UserRole.BQL_Manager && !u.IsDeleted);
        if (approver == null)
        {
            return (false, "Không tìm thấy người duyệt hợp lệ.");
        }

        var invoice = await _invoiceRepository.GetWithDetailsAsync(invoiceId);
        if (invoice == null)
        {
            return (false, "Không tìm thấy hóa đơn.");
        }

        invoice.ApprovalStatus = InvoiceApprovalStatus.Approved;
        invoice.ApprovedBy = approverUserId;
        invoice.ApprovedAt = DateTime.Now;
        invoice.UpdatedAt = DateTime.Now;

        await _invoiceRepository.UpdateAsync(invoice);
        await _activityLogService.LogCustomAsync("ApproveInvoice", nameof(Invoice), invoice.InvoiceId.ToString(), $"Duyệt hóa đơn {invoice.InvoiceNumber}");

        return (true, "Duyệt hóa đơn thành công.");
    }

    public async Task<(bool Success, string Message)> SendInvoiceAsync(int invoiceId)
    {
        var invoice = await _invoiceRepository.GetWithDetailsAsync(invoiceId);
        if (invoice == null)
        {
            return (false, "Không tìm thấy hóa đơn.");
        }

        if (invoice.ApprovalStatus != InvoiceApprovalStatus.Approved)
        {
            return (false, "Chỉ được gửi hóa đơn đã duyệt.");
        }

        if (invoice.IsSent)
        {
            return (false, "Hóa đơn này đã được gửi trước đó.");
        }

        var residents = await _context.Users
            .Where(u => !u.IsDeleted && u.Role == UserRole.Resident && u.ApartmentId == invoice.ApartmentId)
            .ToListAsync();

        foreach (var resident in residents)
        {
            var notification = new Notification
            {
                UserId = resident.UserId,
                Title = $"Hóa đơn tháng {invoice.BillingMonth:D2}/{invoice.BillingYear}",
                Content = $"Hóa đơn {invoice.InvoiceNumber} đã được phát hành với tổng tiền {invoice.TotalAmount:N0} VND.",
                NotificationType = NotificationType.Invoice,
                ReferenceType = ReferenceType.Invoice,
                ReferenceId = invoice.InvoiceId,
                Priority = NotificationPriority.High,
                CreatedAt = DateTime.Now
            };

            await _notificationRepository.AddAsync(notification);

            if (!string.IsNullOrWhiteSpace(resident.Email))
            {
                try
                {
                    await _emailService.SendEmailAsync(
                        resident.Email,
                        $"Hóa đơn {invoice.InvoiceNumber}",
                        $"<p>Xin chào {resident.FullName},</p><p>Hóa đơn <strong>{invoice.InvoiceNumber}</strong> đã được phát hành.</p><p>Tổng tiền: <strong>{invoice.TotalAmount:N0} VND</strong></p><p>Hạn thanh toán: {invoice.DueDate:dd/MM/yyyy}</p>");
                }
                catch
                {
                    // Keep notification flow alive even when email fails.
                }
            }
        }

        invoice.IsSent = true;
        invoice.SentAt = DateTime.Now;
        invoice.UpdatedAt = DateTime.Now;
        await _invoiceRepository.UpdateAsync(invoice);
        await _activityLogService.LogCustomAsync("SendInvoice", nameof(Invoice), invoice.InvoiceId.ToString(), $"Gửi hóa đơn {invoice.InvoiceNumber} cho cư dân");

        return (true, "Đã gửi hóa đơn cho cư dân.");
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
