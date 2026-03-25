using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Services.Interfaces;

public interface IResidentCardService
{
    Task<PagedResult<ResidentCard>> GetPagedAsync(
        string? searchTerm,
        CardStatus? status,
        int? apartmentId,
        int pageIndex,
        int pageSize);

    Task<ResidentCard?> GetByIdAsync(int id);
    Task<ResidentCard?> GetByIdWithDetailsAsync(int id);

    Task<(bool Success, string Message)> CreateAsync(CreateResidentCardDto dto, int creatorId);
    Task<(bool Success, string Message)> UpdateAsync(int id, UpdateResidentCardDto dto);
    Task<(bool Success, string Message)> UpdateStatusAsync(int id, CardStatus status);
    Task<(bool Success, string Message)> DeleteAsync(int id);

    Task<List<Apartment>> GetAvailableApartmentsAsync();
    Task<List<User>> GetAvailableResidentsAsync();
    Task<string> GenerateCardNumberAsync();

    Task<UserSearchResult?> SearchUserAsync(string username);
    Task<List<ApartmentWithType>> GetApartmentsByUserAsync(int userId);
    Task<CardStatsResult> GetStatsAsync();
}

public class CardStatsResult
{
    public int Total { get; set; }
    public int Active { get; set; }
    public int Locked { get; set; }
    public int Lost { get; set; }
    public int Expired { get; set; }
}
