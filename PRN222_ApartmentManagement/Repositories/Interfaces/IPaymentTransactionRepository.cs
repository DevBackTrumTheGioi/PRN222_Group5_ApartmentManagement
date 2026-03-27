using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Repositories.Interfaces;

public interface IPaymentTransactionRepository : IGenericRepository<PaymentTransaction>
{
    Task<PaymentTransaction?> GetWithInvoiceAsync(int transactionId);
    Task<List<PaymentTransaction>> GetHistoryAsync(int? billingMonth, int? billingYear, string? paymentMethod, int? apartmentId = null);
    Task<List<PaymentTransaction>> GetHistoryByApartmentsAsync(int? billingMonth, int? billingYear, string? paymentMethod, List<int> apartmentIds);
    Task<PaymentTransaction?> GetByTxnRefAsync(string txnRef);
}

