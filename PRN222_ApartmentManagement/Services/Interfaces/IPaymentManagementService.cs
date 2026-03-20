using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Services.Interfaces;

public interface IPaymentManagementService
{
    Task<List<Invoice>> GetCollectableInvoicesAsync(int? billingMonth, int? billingYear);
    Task<List<PaymentTransaction>> GetManagementTransactionsAsync(int? billingMonth, int? billingYear, string? paymentMethod);
    Task<List<Invoice>> GetResidentPayableInvoicesAsync(int residentUserId, int? billingMonth, int? billingYear);
    Task<List<PaymentTransaction>> GetResidentTransactionsAsync(int residentUserId, int? billingMonth, int? billingYear, string? paymentMethod);
    Task<(bool Success, string Message)> RecordCashPaymentAsync(int invoiceId, decimal amount, int staffUserId, string? note);
    Task<(bool Success, string Message)> ConfirmBankTransferAsync(int invoiceId, decimal amount, int staffUserId, string referenceCode, string? note);
    Task<(bool Success, string Message)> CreateOnlinePaymentAsync(int invoiceId, int residentUserId, string gateway);
    Task<PaymentTransaction?> GetReceiptForManagementAsync(int transactionId);
    Task<PaymentTransaction?> GetReceiptForResidentAsync(int transactionId, int residentUserId);
}
