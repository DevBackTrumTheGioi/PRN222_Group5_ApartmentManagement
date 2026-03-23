using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services;

namespace PRN222_ApartmentManagement.Repositories.Implementations;

public class ResidentApartmentRepository : GenericRepository<ResidentApartment>, IResidentApartmentRepository
{
    public ResidentApartmentRepository(ApartmentDbContext context, IActivityLogService activityLog)
        : base(context, activityLog)
    {
    }

    public async Task<List<ResidentApartment>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(ra => ra.UserId == userId)
            .Include(ra => ra.Apartment)
            .Include(ra => ra.Contract)
            .OrderByDescending(ra => ra.IsActive)
            .ThenByDescending(ra => ra.CreatedAt)
            .ToListAsync();
    }

    public async Task<ResidentApartment?> GetPrimaryAsync(int userId)
    {
        return await _dbSet
            .Where(ra => ra.UserId == userId && ra.IsActive)
            .Include(ra => ra.Apartment)
            .FirstOrDefaultAsync();
    }

    public async Task DeactivateAllForUserAsync(int userId)
    {
        var records = await _dbSet
            .Where(ra => ra.UserId == userId && ra.IsActive)
            .ToListAsync();

        foreach (var record in records)
        {
            record.IsActive = false;
            record.MoveOutDate = DateTime.Now;
            record.UpdatedAt = DateTime.Now;
        }

        await _context.SaveChangesAsync();
    }
}
