using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Repositories.Interfaces;

public interface IApartmentRepository
{
    Task<IEnumerable<Apartment>> GetAllAsync();
    Task<Apartment?> GetByIdAsync(int id);
    Task AddAsync(Apartment apartment);
    Task UpdateAsync(Apartment apartment);
    Task DeleteAsync(int id);
}

