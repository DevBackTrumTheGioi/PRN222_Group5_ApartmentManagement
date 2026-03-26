using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services;

namespace PRN222_ApartmentManagement.Repositories.Implementations;

/// <summary>
/// Repository implementation for ServiceOrder entity
/// </summary>
public class ServiceOrderRepository : GenericRepository<ServiceOrder>, IServiceOrderRepository
{
    public ServiceOrderRepository(ApartmentDbContext context, IActivityLogService activityLog)
        : base(context, activityLog)
    {
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ServiceOrder>> GetByApartmentIdAsync(int apartmentId)
    {
        return await _dbSet
            .Include(so => so.ServiceType)
            .Include(so => so.Resident)
            .Where(so => so.ApartmentId == apartmentId)
            .OrderByDescending(so => so.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ServiceOrder>> GetByResidentIdAsync(int residentId)
    {
        return await _dbSet
            .Include(so => so.ServiceType)
            .Include(so => so.Apartment)
            .Include(so => so.Resident)
            .Where(so => so.ResidentId == residentId)
            .OrderByDescending(so => so.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ServiceOrder>> GetByStatusAsync(ServiceOrderStatus status)
    {
        return await _dbSet
            .Include(so => so.ServiceType)
            .Include(so => so.Apartment)
            .Include(so => so.Resident)
            .Where(so => so.Status == status)
            .OrderByDescending(so => so.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ServiceOrder>> GetByAssignedStaffAsync(int staffId)
    {
        return await _dbSet
            .Include(so => so.ServiceType)
            .Include(so => so.Apartment)
            .Include(so => so.Resident)
            .Where(so => so.AssignedTo == staffId)
            .OrderByDescending(so => so.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ServiceOrder>> GetByServiceTypeAsync(int serviceTypeId)
    {
        return await _dbSet
            .Include(so => so.Apartment)
            .Include(so => so.Resident)
            .Where(so => so.ServiceTypeId == serviceTypeId)
            .OrderByDescending(so => so.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ServiceOrder>> GetByRequestedDateAsync(DateTime date)
    {
        return await _dbSet
            .Include(so => so.ServiceType)
            .Include(so => so.Apartment)
            .Include(so => so.Resident)
            .Where(so => so.RequestedDate.Date == date.Date)
            .OrderBy(so => so.RequestedTimeSlot)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ServiceOrder>> GetPendingOrdersAsync()
    {
        return await _dbSet
            .Include(so => so.ServiceType)
            .Include(so => so.Apartment)
            .Include(so => so.Resident)
            .Where(so => so.Status == ServiceOrderStatus.Pending)
            .OrderBy(so => so.RequestedDate)
            .ThenBy(so => so.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<string> GenerateOrderNumberAsync()
    {
        var today = DateTime.Now;
        var prefix = $"SO-{today:yyyyMMdd}-";

        var lastOrder = await _dbSet
            .Where(so => so.OrderNumber.StartsWith(prefix))
            .OrderByDescending(so => so.OrderNumber)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (lastOrder != null)
        {
            var lastNumberStr = lastOrder.OrderNumber.Replace(prefix, "");
            if (int.TryParse(lastNumberStr, out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"{prefix}{nextNumber:D3}";
    }

    /// <inheritdoc/>
    public async Task<ServiceOrder?> GetWithDetailsAsync(int serviceOrderId)
    {
        return await _dbSet
            .Include(so => so.Apartment)
            .Include(so => so.Resident)
            .Include(so => so.ServiceType)
            .Include(so => so.AssignedStaff)
            .Include(so => so.CompletedByUser)
            .Include(so => so.Invoice)
            .FirstOrDefaultAsync(so => so.ServiceOrderId == serviceOrderId);
    }
}
