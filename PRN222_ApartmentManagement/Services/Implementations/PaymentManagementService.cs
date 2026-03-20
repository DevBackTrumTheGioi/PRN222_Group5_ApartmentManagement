using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services.Interfaces;
using PRN222_ApartmentManagement.Utils;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class PaymentManagementService : IPaymentManagementService
{
    private readonly ApartmentDbContext _context;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IPaymentTransactionRepository _paymentTransactionRepository;
    private readonly IActivityLogService _activityLogService;

    public PaymentManagementService(
        ApartmentDbContext context,
        IInvoiceRepository invoiceRepository,
        IPaymentTransactionRepository paymentTransactionRepository,
        IActivityLogService activityLogService)
    {
        _context = context;
        _invoiceRepository = invoiceRepository;
        _paymentTransactionRepository = paymentTransactionRepository;
        _activityLogService = activityLogService;
    }

    public async Task<List<Invoice>> GetCollectableInvoicesAsync(int? billingMonth, int? billingYear)
    {
        var invoices = await _invoiceRepository.GetFilteredAsync(billingMonth, billingYear, null, null);

        return invoices
            .Where(i =>
                i.ApprovalStatus == InvoiceApprovalStatus.Approved &&
                i.Status != InvoiceStatus.Paid &&
                i.Status != InvoiceStatus.Cancelled)
            .OrderByDescending(i => i.BillingYear)
            .ThenByDescending(i => i.BillingMonth)
            .ThenByDescending(i => i.CreatedAt)
            .ToList();
    }

    public Task<List<PaymentTransaction>> GetManagementTransactionsAsync(int? billingMonth, int? billingYear, string? paymentMethod)
    {
        return _paymentTransactionRepository.GetHistoryAsync(billingMonth, billingYear, paymentMethod);
    }

    public async Task<List<Invoice>> GetResidentPayableInvoicesAsync(int residentUserId, int? billingMonth, int? billingYear)
    {
        var resident = await GetResidentAsync(residentUserId);
        if (resident?.ApartmentId == null)
        {
            return new List<Invoice>();
        }

        var invoices = await _invoiceRepository.GetByApartmentAsync(resident.ApartmentId.Value, billingMonth, billingYear);

        return invoices
            .Where(i =>
                i.ApprovalStatus == InvoiceApprovalStatus.Approved &&
                i.IsSent &&
                i.Status != InvoiceStatus.Paid &&
                i.Status != InvoiceStatus.Cancelled)
            .ToList();
    }

    public async Task<List<PaymentTransaction>> GetResidentTransactionsAsync(int residentUserId, int? billingMonth, int? billingYear, string? paymentMethod)
    {
        var resident = await GetResidentAsync(residentUserId);
        if (resident?.ApartmentId == null)
        {
            return new List<PaymentTransaction>();
        }

        return await _paymentTransactionRepository.GetHistoryAsync(billingMonth, billingYear, paymentMethod, resident.ApartmentId.Value);
    }

    public Task<PaymentTransaction?> GetReceiptForManagementAsync(int transactionId)
    {
        return _paymentTransactionRepository.GetWithInvoiceAsync(transactionId);
    }

    public async Task<PaymentTransaction?> GetReceiptForResidentAsync(int transactionId, int residentUserId)
    {
        var resident = await GetResidentAsync(residentUserId);
        if (resident?.ApartmentId == null)
        {
            return null;
        }

        var transaction = await _paymentTransactionRepository.GetWithInvoiceAsync(transactionId);
        if (transaction == null || transaction.Invoice.ApartmentId != resident.ApartmentId.Value)
        {
            return null;
        }

        return transaction;
    }

    public Task<(bool Success, string Message)> RecordCashPaymentAsync(int invoiceId, decimal amount, int staffUserId, string? note)
    {
        return RecordPaymentAsync(invoiceId, amount, "Cash", staffUserId, note, StringUtils.GenerateTransactionCode(), "Thu tiền mặt", true);
    }

    public Task<(bool Success, string Message)> ConfirmBankTransferAsync(int invoiceId, decimal amount, int staffUserId, string referenceCode, string? note)
    {
        var transactionCode = string.IsNullOrWhiteSpace(referenceCode)
            ? StringUtils.GenerateTransactionCode()
            : referenceCode.Trim();

        return RecordPaymentAsync(invoiceId, amount, "BankTransfer", staffUserId, note, transactionCode, "Xác nhận chuyển khoản", true);
    }

    public async Task<(bool Success, string Message)> CreateOnlinePaymentAsync(int invoiceId, int residentUserId, string gateway)
    {
        var resident = await GetResidentAsync(residentUserId);
        if (resident?.ApartmentId == null)
        {
            return (false, "Không tìm thấy cư dân hợp lệ.");
        }

        var invoice = await _invoiceRepository.GetWithDetailsAsync(invoiceId);
        if (invoice == null || invoice.ApartmentId != resident.ApartmentId.Value)
        {
            return (false, "Không tìm thấy hóa đơn phù hợp.");
        }

        var remainingAmount = invoice.TotalAmount - invoice.PaidAmount;
        if (remainingAmount <= 0)
        {
            return (false, "Hóa đơn này đã được thanh toán đủ.");
        }

        var selectedGateway = string.IsNullOrWhiteSpace(gateway) ? "VNPay" : gateway.Trim();

        return await RecordPaymentAsync(
            invoiceId,
            remainingAmount,
            selectedGateway,
            residentUserId,
            $"Thanh toán online mô phỏng qua {selectedGateway}",
            StringUtils.GenerateTransactionCode(),
            "Thanh toán online",
            false);
    }

    private async Task<(bool Success, string Message)> RecordPaymentAsync(
        int invoiceId,
        decimal amount,
        string paymentMethod,
        int actorUserId,
        string? note,
        string transactionCode,
        string activityName,
        bool createdByStaff)
    {
        if (amount <= 0)
        {
            return (false, "Số tiền thanh toán phải lớn hơn 0.");
        }

        var invoice = await _invoiceRepository.GetWithDetailsAsync(invoiceId);
        if (invoice == null)
        {
            return (false, "Không tìm thấy hóa đơn.");
        }

        if (invoice.ApprovalStatus != InvoiceApprovalStatus.Approved)
        {
            return (false, "Chỉ có thể thanh toán hóa đơn đã được duyệt.");
        }

        if (invoice.Status == InvoiceStatus.Cancelled)
        {
            return (false, "Không thể thanh toán hóa đơn đã hủy.");
        }

        var remainingAmount = invoice.TotalAmount - invoice.PaidAmount;
        if (remainingAmount <= 0)
        {
            return (false, "Hóa đơn này đã được thanh toán đủ.");
        }

        if (amount > remainingAmount)
        {
            return (false, $"Số tiền vượt quá phần còn lại cần thanh toán ({remainingAmount:N0} VNĐ).");
        }

        var transaction = new PaymentTransaction
        {
            InvoiceId = invoice.InvoiceId,
            TransactionCode = transactionCode,
            Amount = amount,
            PaymentMethod = paymentMethod,
            PaymentDate = DateTime.Now,
            Status = (int)PaymentTransactionStatus.Success,
            GatewayResponse = note,
            CreatedBy = actorUserId,
            CreatedAt = DateTime.Now
        };

        await _paymentTransactionRepository.AddAsync(transaction);

        invoice.PaidAmount += amount;
        invoice.PaymentMethod = paymentMethod;
        invoice.PaymentDate = transaction.PaymentDate;
        invoice.Status = GetInvoiceStatus(invoice.TotalAmount, invoice.PaidAmount, invoice.DueDate);
        invoice.UpdatedAt = DateTime.Now;

        await _invoiceRepository.UpdateAsync(invoice);

        var actorLabel = createdByStaff ? "nhân viên" : "cư dân";
        await _activityLogService.LogCustomAsync(
            activityName,
            nameof(PaymentTransaction),
            transaction.TransactionId.ToString(),
            $"{actorLabel} ghi nhận thanh toán {paymentMethod} cho hóa đơn {invoice.InvoiceNumber}");

        return (true, $"Đã ghi nhận thanh toán {amount:N0} VNĐ cho hóa đơn {invoice.InvoiceNumber}.");
    }

    private static InvoiceStatus GetInvoiceStatus(decimal totalAmount, decimal paidAmount, DateTime dueDate)
    {
        if (paidAmount >= totalAmount)
        {
            return InvoiceStatus.Paid;
        }

        if (paidAmount > 0)
        {
            return InvoiceStatus.PartiallyPaid;
        }

        if (DateTime.Now.Date > dueDate.Date)
        {
            return InvoiceStatus.Overdue;
        }

        return InvoiceStatus.Unpaid;
    }

    private Task<User?> GetResidentAsync(int residentUserId)
    {
        return _context.Users.FirstOrDefaultAsync(u =>
            u.UserId == residentUserId &&
            u.Role == UserRole.Resident &&
            !u.IsDeleted);
    }
}
