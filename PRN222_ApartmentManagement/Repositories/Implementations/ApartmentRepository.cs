﻿using Microsoft.EntityFrameworkCore;
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
        return await _context.Apartments.ToListAsync();
    }

    public async Task<Apartment?> GetByIdAsync(int id)
    {
        return await _context.Apartments.FindAsync(id);
    }

    public async Task AddAsync(Apartment apartment)
    {
        _context.Apartments.Add(apartment);
        await _context.SaveChangesAsync();

        // Log CREATE action
        await _activityLog.LogCreateAsync(
            entityName: "Apartment",
            entityId: apartment.ApartmentId.ToString(),
            newValues: apartment,
            description: $"Tạo căn hộ mới: {apartment.ApartmentNumber} (Tầng {apartment.Floor})"
        );
    }

    public async Task UpdateAsync(Apartment apartment)
    {
        // Get old values for logging
        var oldApartment = await _context.Apartments.AsNoTracking()
            .FirstOrDefaultAsync(a => a.ApartmentId == apartment.ApartmentId);

        _context.Entry(apartment).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        // Log UPDATE action
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
        if (apartment != null)
        {
            _context.Apartments.Remove(apartment);
            await _context.SaveChangesAsync();

            // Log DELETE action
            await _activityLog.LogDeleteAsync(
                entityName: "Apartment",
                entityId: id.ToString(),
                oldValues: apartment,
                description: $"Xóa căn hộ: {apartment.ApartmentNumber}"
            );
        }
    }
}

