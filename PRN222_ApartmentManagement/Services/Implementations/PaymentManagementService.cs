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
    private readonly VNPayHelper _vnPayHelper;

    public PaymentManagementService(
        ApartmentDbContext context,
        IInvoiceRepository invoiceRepository,
        IPaymentTransactionRepository paymentTransactionRepository,
        IActivityLogService activityLogService,
        VNPayHelper vnPayHelper)
    {
        _context = context;
        _invoiceRepository = invoiceRepository;
        _paymentTransactionRepository = paymentTransactionRepository;
        _activityLogService = activityLogService;
        _vnPayHelper = vnPayHelper;
    }

    public async Task<List<Invoice>> GetCollectableInvoicesAsync(int? billingMonth, int? billingYear)
    {
        var invoices = await _invoiceRepository.GetFilteredAsync(billingMonth, billingYear, null);

        return invoices
            .Where(i =>
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
        var apartmentIds = await GetResidentApartmentIdsAsync(residentUserId);
        if (!apartmentIds.Any()) return new List<Invoice>();

        var invoices = await _invoiceRepository.GetByApartmentsAsync(apartmentIds, billingMonth, billingYear);

        return invoices
            .Where(i => i.IsSent
                        && i.Status != InvoiceStatus.Paid
                        && i.Status != InvoiceStatus.Cancelled)
            .ToList();
    }

    public async Task<List<PaymentTransaction>> GetResidentTransactionsAsync(int residentUserId, int? billingMonth, int? billingYear, string? paymentMethod)
    {
        var apartmentIds = await GetResidentApartmentIdsAsync(residentUserId);
        if (!apartmentIds.Any()) return new List<PaymentTransaction>();

        return await _paymentTransactionRepository.GetHistoryByApartmentsAsync(billingMonth, billingYear, paymentMethod, apartmentIds);
    }

    public Task<PaymentTransaction?> GetReceiptForManagementAsync(int transactionId)
    {
        return _paymentTransactionRepository.GetWithInvoiceAsync(transactionId);
    }

    public async Task<PaymentTransaction?> GetReceiptForResidentAsync(int transactionId, int residentUserId)
    {
        var apartmentIds = await GetResidentApartmentIdsAsync(residentUserId);
        if (!apartmentIds.Any()) return null;

        var transaction = await _paymentTransactionRepository.GetWithInvoiceAsync(transactionId);
        if (transaction == null || !apartmentIds.Contains(transaction.Invoice.ApartmentId))
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
        var apartmentIds = await GetResidentApartmentIdsAsync(residentUserId);
        if (!apartmentIds.Any())
        {
            return (false, "Không tìm thấy cư dân hợp lệ.");
        }

        var invoice = await _invoiceRepository.GetWithDetailsAsync(invoiceId);
        if (invoice == null || !apartmentIds.Contains(invoice.ApartmentId))
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

    public async Task<(bool Success, string PaymentUrl, string Message)> CreateOnlinePaymentRequestAsync(
        int invoiceId, int residentUserId)
    {
        var apartmentIds = await GetResidentApartmentIdsAsync(residentUserId);
        if (!apartmentIds.Any())
        {
            return (false, "", "Không tìm thấy cư dân hợp lệ.");
        }

        var invoice = await _invoiceRepository.GetWithDetailsAsync(invoiceId);
        if (invoice == null || !apartmentIds.Contains(invoice.ApartmentId))
        {
            return (false, "", "Không tìm thấy hóa đơn phù hợp.");
        }

        if (invoice.Status == InvoiceStatus.Cancelled)
        {
            return (false, "", "Không thể thanh toán hóa đơn đã hủy.");
        }

        var remainingAmount = invoice.TotalAmount - invoice.PaidAmount;
        if (remainingAmount <= 0)
        {
            return (false, "", "Hóa đơn này đã được thanh toán đủ.");
        }

        var txnRef = $"INV-{invoiceId}-{DateTimeOffset.Now.ToUnixTimeSeconds()}";

        var existingPending = await _context.PaymentTransactions
            .Where(t => t.InvoiceId == invoiceId && t.PaymentMethod == "VNPay" && t.Status == (int)PaymentTransactionStatus.Pending)
            .FirstOrDefaultAsync();

        int transactionId;
        if (existingPending != null)
        {
            transactionId = existingPending.TransactionId;
            existingPending.VnpTxnRef = txnRef;
            existingPending.Amount = remainingAmount;
            existingPending.PaymentDate = DateTime.Now;
            existingPending.GatewayResponse = "Yêu cầu thanh toán được tạo lại";
            await _paymentTransactionRepository.UpdateAsync(existingPending);
        }
        else
        {
            var transaction = new PaymentTransaction
            {
                InvoiceId = invoiceId,
                TransactionCode = StringUtils.GenerateTransactionCode(),
                Amount = remainingAmount,
                PaymentMethod = "VNPay",
                PaymentDate = DateTime.Now,
                Status = (int)PaymentTransactionStatus.Pending,
                VnpTxnRef = txnRef,
                CreatedBy = residentUserId,
                CreatedAt = DateTime.Now
            };
            await _paymentTransactionRepository.AddAsync(transaction);
            transactionId = transaction.TransactionId;
        }

        var paymentUrl = _vnPayHelper.CreatePaymentUrl(
            invoiceId,
            remainingAmount,
            invoice.InvoiceNumber,
            txnRef);

        return (true, paymentUrl, "Đang chuyển hướng đến cổng thanh toán VNPay.");
    }

    public async Task<(bool Success, string Message)> ProcessPaymentCallbackAsync(
        string txnRef, string responseCode, string transactionNo,
        string bankCode, string payDate, string secureHash,
        Dictionary<string, string>? allParams = null)
    {
        if (allParams != null && allParams.Count > 0)
        {
            if (!_vnPayHelper.VerifySignature(allParams))
            {
                return (false, "Chữ ký không hợp lệ.");
            }
        }
        else
        {
            var parameters = new Dictionary<string, string>
            {
                ["vnp_TxnRef"] = txnRef,
                ["vnp_ResponseCode"] = responseCode,
                ["vnp_TransactionNo"] = transactionNo,
                ["vnp_BankCode"] = bankCode,
                ["vnp_PayDate"] = payDate,
                ["vnp_SecureHash"] = secureHash
            };
            if (!_vnPayHelper.VerifySignature(parameters))
            {
                return (false, "Chữ ký không hợp lệ.");
            }
        }

        var transaction = await _paymentTransactionRepository.GetByTxnRefAsync(txnRef);
        if (transaction == null)
        {
            return (false, "Không tìm thấy giao dịch.");
        }

        if (transaction.Status == (int)PaymentTransactionStatus.Success)
        {
            return (true, "Giao dịch đã được xác nhận trước đó.");
        }

        if (responseCode == "00")
        {
            transaction.Status = (int)PaymentTransactionStatus.Success;
            transaction.GatewayResponse = $"Thanh toán thành công qua VNPay. Mã GD VNPay: {transactionNo}";

            var invoice = await _invoiceRepository.GetWithDetailsAsync(transaction.InvoiceId);
            if (invoice != null)
            {
                invoice.PaidAmount += transaction.Amount;
                invoice.PaymentMethod = "VNPay";
                invoice.PaymentDate = DateTime.Now;
                invoice.Status = GetInvoiceStatus(invoice.TotalAmount, invoice.PaidAmount, invoice.DueDate);
                invoice.UpdatedAt = DateTime.Now;
            }

            await _activityLogService.LogCustomAsync(
                "VNPayPaymentSuccess",
                nameof(PaymentTransaction),
                transaction.TransactionId.ToString(),
                $"Thanh toán VNPay thành công {transaction.Amount:N0} VND cho hóa đơn");

            return (true, "Xác nhận thành công");
        }
        else
        {
            transaction.Status = (int)PaymentTransactionStatus.Failed;
            transaction.GatewayResponse = $"Thanh toán thất bại qua VNPay. Mã lỗi: {responseCode}";

            await _activityLogService.LogCustomAsync(
                "VNPayPaymentFailed",
                nameof(PaymentTransaction),
                transaction.TransactionId.ToString(),
                $"Thanh toán VNPay thất bại. Mã lỗi: {responseCode}");

            return (true, $"Thanh toán thất bại. Mã lỗi: {responseCode}");
        }
    }

    private async Task<List<int>> GetResidentApartmentIdsAsync(int residentUserId)
    {
        return await _context.ResidentApartments
            .Where(ra => ra.UserId == residentUserId && ra.IsActive)
            .Select(ra => ra.ApartmentId)
            .ToListAsync();
    }

    private async Task<User?> GetResidentAsync(int residentUserId)
    {
        return await _context.Users.FirstOrDefaultAsync(u =>
            u.UserId == residentUserId &&
            u.Role == UserRole.Resident &&
            !u.IsDeleted);
    }
}
