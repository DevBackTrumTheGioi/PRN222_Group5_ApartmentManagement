using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Repositories.Interfaces;

public interface IResidentApartmentRepository : IGenericRepository<ResidentApartment>
{
    Task<List<ResidentApartment>> GetByUserIdAsync(int userId);
    Task<ResidentApartment?> GetPrimaryAsync(int userId);
    Task DeactivateAllForUserAsync(int userId);
}
