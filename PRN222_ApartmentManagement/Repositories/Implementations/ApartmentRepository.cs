using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services;

namespace PRN222_ApartmentManagement.Repositories.Implementations;

/// <summary>
/// Repository implementation for Apartment entity with Activity Logging
/// </summary>
public class ApartmentRepository : IApartmentRepository
{
    private readonly ApartmentDbContext _context;
    private readonly IActivityLogService _activityLog;

    public ApartmentRepository(ApartmentDbContext context, IActivityLogService activityLog)
    {
        _context = context;
        _activityLog = activityLog;
    }

    public async Task<IEnumerable<Apartment>> GetAllAsync()
    {
        return await _context.Apartments
            .Where(a => !a.IsDeleted)
            .ToListAsync();
    }

    public async Task<Apartment?> GetByIdAsync(int id)
    {
        return await _context.Apartments
            .Where(a => !a.IsDeleted)
            .FirstOrDefaultAsync(a => a.ApartmentId == id);
    }

    public async Task AddAsync(Apartment apartment)
    {
        _context.Apartments.Add(apartment);
        await _context.SaveChangesAsync();

        await _activityLog.LogCreateAsync(
            entityName: "Apartment",
            entityId: apartment.ApartmentId.ToString(),
            newValues: apartment,
            description: $"Tạo căn hộ mới: {apartment.ApartmentNumber} (Tầng {apartment.Floor})"
        );
    }

    public async Task UpdateAsync(Apartment apartment)
    {
        var oldApartment = await _context.Apartments.AsNoTracking()
            .Where(a => !a.IsDeleted)
            .FirstOrDefaultAsync(a => a.ApartmentId == apartment.ApartmentId);

        _context.Entry(apartment).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        await _activityLog.LogUpdateAsync(
            entityName: "Apartment",
            entityId: apartment.ApartmentId.ToString(),
            oldValues: oldApartment!,
            newValues: apartment,
            description: $"Cập nhật căn hộ: {apartment.ApartmentNumber}"
        );
    }

    public async Task DeleteAsync(int id)
    {
        var apartment = await _context.Apartments.FindAsync(id);
        if (apartment != null && !apartment.IsDeleted)
        {
            apartment.IsDeleted = true;
            apartment.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            await _activityLog.LogDeleteAsync(
                entityName: "Apartment",
                entityId: id.ToString(),
                oldValues: apartment,
                description: $"Xóa mềm căn hộ: {apartment.ApartmentNumber}"
            );
        }
    }

    public async Task<IEnumerable<Apartment>> FindAsync(Expression<Func<Apartment, bool>> predicate)
    {
        return await _context.Apartments
            .Where(a => !a.IsDeleted)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Apartment> entities)
    {
        await _context.Apartments.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Apartment entity)
    {
        if (!entity.IsDeleted)
        {
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            await _activityLog.LogDeleteAsync(
                entityName: "Apartment",
                entityId: entity.ApartmentId.ToString(),
                oldValues: entity,
                description: $"Xóa mềm căn hộ: {entity.ApartmentNumber}"
            );
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Apartments
            .Where(a => !a.IsDeleted)
            .AnyAsync(a => a.ApartmentId == id);
    }

    public async Task<IEnumerable<Apartment>> GetAvailableApartmentsAsync()
    {
        var occupiedApartmentIds = await _context.ResidentApartments
            .Where(ra => ra.IsActive)
            .Select(ra => ra.ApartmentId)
            .Distinct()
            .ToListAsync();

        return await _context.Apartments
            .Where(a => !a.IsDeleted && !occupiedApartmentIds.Contains(a.ApartmentId))
            .ToListAsync();
    }
}
