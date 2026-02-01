using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Repositories.Interfaces;

namespace PRN222_ApartmentManagement.Repositories.Implementations;

/// <summary>
/// Repository implementation for Apartment entity
/// </summary>
public class ApartmentRepository : IApartmentRepository
{
    private readonly ApartmentDbContext _context;

    public ApartmentRepository(ApartmentDbContext context)
    {
        _context = context;
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
    }

    public async Task UpdateAsync(Apartment apartment)
    {
        _context.Entry(apartment).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var apartment = await _context.Apartments.FindAsync(id);
        if (apartment != null)
        {
            _context.Apartments.Remove(apartment);
            await _context.SaveChangesAsync();
        }
    }
}

