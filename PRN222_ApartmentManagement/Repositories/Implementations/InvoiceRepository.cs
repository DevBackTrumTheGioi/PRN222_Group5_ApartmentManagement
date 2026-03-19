using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services;

namespace PRN222_ApartmentManagement.Repositories.Implementations;

public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(ApartmentDbContext context, IActivityLogService activityLog)
        : base(context, activityLog)
    {
    }

    public async Task<bool> ExistsForApartmentMonthAsync(int apartmentId, int billingMonth, int billingYear)
    {
        return await _dbSet.AnyAsync(i =>
            i.ApartmentId == apartmentId &&
            i.BillingMonth == billingMonth &&
            i.BillingYear == billingYear);
    }

    public async Task<string> GenerateInvoiceNumberAsync(int billingMonth, int billingYear, int apartmentId)
    {
        var prefix = $"INV-{billingYear}{billingMonth:D2}-{apartmentId:D4}-";
        var lastInvoice = await _dbSet
            .Where(i => i.InvoiceNumber.StartsWith(prefix))
            .OrderByDescending(i => i.InvoiceNumber)
            .FirstOrDefaultAsync();

        var nextNumber = 1;
        if (lastInvoice != null)
        {
            var suffix = lastInvoice.InvoiceNumber.Replace(prefix, string.Empty);
            if (int.TryParse(suffix, out var parsed))
            {
                nextNumber = parsed + 1;
            }
        }

        return $"{prefix}{nextNumber:D3}";
    }

    public async Task<Invoice?> GetWithDetailsAsync(int invoiceId)
    {
        return await _dbSet
            .Include(i => i.Apartment)
            .Include(i => i.Creator)
            .Include(i => i.Approver)
            .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.ServiceType)
            .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.ServicePrice)
            .Include(i => i.PaymentTransactions)
            .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
    }

    public async Task<List<Invoice>> GetFilteredAsync(
        int? billingMonth,
        int? billingYear,
        InvoiceStatus? status,
        InvoiceApprovalStatus? approvalStatus)
    {
        var query = _dbSet
            .Include(i => i.Apartment)
            .Include(i => i.Creator)
            .Include(i => i.Approver)
            .AsQueryable();

        if (billingMonth.HasValue)
        {
            query = query.Where(i => i.BillingMonth == billingMonth.Value);
        }

        if (billingYear.HasValue)
        {
            query = query.Where(i => i.BillingYear == billingYear.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(i => i.Status == status.Value);
        }

        if (approvalStatus.HasValue)
        {
            query = query.Where(i => i.ApprovalStatus == approvalStatus.Value);
        }

        return await query
            .OrderByDescending(i => i.BillingYear)
            .ThenByDescending(i => i.BillingMonth)
            .ThenByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Invoice>> GetByApartmentAsync(int apartmentId, int? billingMonth = null, int? billingYear = null)
    {
        var query = _dbSet
            .Include(i => i.Apartment)
            .Include(i => i.Creator)
            .Include(i => i.Approver)
            .Where(i => i.ApartmentId == apartmentId);

        if (billingMonth.HasValue)
        {
            query = query.Where(i => i.BillingMonth == billingMonth.Value);
        }

        if (billingYear.HasValue)
        {
            query = query.Where(i => i.BillingYear == billingYear.Value);
        }

        return await query
            .OrderByDescending(i => i.BillingYear)
            .ThenByDescending(i => i.BillingMonth)
            .ThenByDescending(i => i.CreatedAt)
            .ToListAsync();
    }
}
