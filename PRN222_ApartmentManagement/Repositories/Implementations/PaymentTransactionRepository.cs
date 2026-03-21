using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace PRN222_ApartmentManagement.Repositories.Implementations;

public class PaymentTransactionRepository : GenericRepository<PaymentTransaction>, IPaymentTransactionRepository
{
    public PaymentTransactionRepository(ApartmentDbContext context, IActivityLogService activityLog) : base(context, activityLog)
    {
    }

    public async Task<PaymentTransaction?> GetWithInvoiceAsync(int transactionId)
    {
        return await _dbSet
            .Include(t => t.Creator)
            .Include(t => t.Invoice)
                .ThenInclude(i => i.Apartment)
            .Include(t => t.Invoice)
                .ThenInclude(i => i.Creator)
            .Include(t => t.Invoice)
                .ThenInclude(i => i.Approver)
            .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
    }

    public async Task<List<PaymentTransaction>> GetHistoryAsync(int? billingMonth, int? billingYear, string? paymentMethod, int? apartmentId = null)
    {
        var query = _dbSet
            .Include(t => t.Creator)
            .Include(t => t.Invoice)
                .ThenInclude(i => i.Apartment)
            .AsQueryable();

        if (billingMonth.HasValue)
        {
            query = query.Where(t => t.Invoice.BillingMonth == billingMonth.Value);
        }

        if (billingYear.HasValue)
        {
            query = query.Where(t => t.Invoice.BillingYear == billingYear.Value);
        }

        if (!string.IsNullOrWhiteSpace(paymentMethod))
        {
            query = query.Where(t => t.PaymentMethod == paymentMethod);
        }

        if (apartmentId.HasValue)
        {
            query = query.Where(t => t.Invoice.ApartmentId == apartmentId.Value);
        }

        return await query
            .OrderByDescending(t => t.PaymentDate)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}

