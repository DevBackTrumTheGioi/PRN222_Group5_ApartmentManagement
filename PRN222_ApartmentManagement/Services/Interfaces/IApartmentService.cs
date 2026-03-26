using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Services.Interfaces;

public interface IApartmentService
{
    Task<PagedResult<Apartment>> GetPagedAsync(
        string? searchTerm,
        ApartmentStatus? status,
        string? type,
        string? block,
        int? floor,
        int pageIndex,
        int pageSize);

    Task<ApartmentDetailDto?> GetDetailAsync(int id);

    Task<(bool Success, string Message)> CreateAsync(CreateApartmentDto dto, int creatorId);
    Task<(bool Success, string Message)> UpdateAsync(int id, UpdateApartmentDto dto);
    Task<(bool Success, string Message)> DeleteAsync(int id);

    Task<ApartmentStatsResult> GetStatsAsync();
    Task<List<Apartment>> GetAllAsync();
}
