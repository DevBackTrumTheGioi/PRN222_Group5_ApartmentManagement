using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;

namespace PRN222_ApartmentManagement.Services.Interfaces;

public interface IResidentApartmentAccessService
{
    Task<List<int>> GetActiveApartmentIdsAsync(int residentUserId);
    Task<List<ApartmentWithType>> GetActiveApartmentOptionsAsync(int residentUserId);
    Task<bool> HasAnyActiveApartmentAsync(int residentUserId);
    Task<bool> IsResidentInApartmentAsync(int residentUserId, int apartmentId);
    Task<Apartment?> GetPreferredApartmentAsync(int residentUserId);
}
