using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Repositories.Interfaces;

public interface IResidentCardRepository : IGenericRepository<ResidentCard>
{
    Task<PagedResult<ResidentCard>> GetPagedAsync(
        string? searchTerm,
        CardStatus? status,
        int? apartmentId,
        int pageIndex,
        int pageSize);

    Task<ResidentCard?> GetByIdWithDetailsAsync(int id);

    Task<string> GenerateCardNumberAsync();

    Task<IEnumerable<ResidentCard>> GetAllAsync();
}
