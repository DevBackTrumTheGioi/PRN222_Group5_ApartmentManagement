using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Repositories.Interfaces;

public interface IInvoiceRepository : IGenericRepository<Invoice>
{
    Task<bool> ExistsForApartmentMonthAsync(int apartmentId, int billingMonth, int billingYear);
    Task<string> GenerateInvoiceNumberAsync(int billingMonth, int billingYear, int apartmentId);
    Task<Invoice?> GetWithDetailsAsync(int invoiceId);
    Task<List<Invoice>> GetFilteredAsync(int? billingMonth, int? billingYear, InvoiceStatus? status);
    Task<List<Invoice>> GetByApartmentAsync(int apartmentId, int? billingMonth = null, int? billingYear = null);
    Task<List<Invoice>> GetByApartmentsAsync(List<int> apartmentIds, int? billingMonth = null, int? billingYear = null);
}
