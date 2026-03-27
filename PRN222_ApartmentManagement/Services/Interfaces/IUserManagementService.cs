using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;

namespace PRN222_ApartmentManagement.Services.Interfaces;

public interface IUserManagementService
{
    Task<List<Apartment>> GetAssignableApartmentsAsync();
    Task<(List<User> Users, int TotalItems, int TotalPages)> GetPagedUsersAsync(string? searchTerm, string? roleFilter, int pageIndex, int pageSize);
    Task<User?> GetUserForEditAsync(int userId);
    Task<(bool Success, string? ErrorField, string? ErrorMessage)> CreateUserAsync(UserUpsertRequest request);
    Task<(bool Success, string? ErrorField, string? ErrorMessage)> UpdateUserAsync(UserUpsertRequest request);
    Task<(bool Success, string? ErrorField, string? ErrorMessage)> CreateFamilyMemberAsync(
        FamilyMemberCreateDto dto,
        int creatorResidentUserId,
        int apartmentId);
    Task<(bool Success, string? ErrorMessage)> ToggleStatusAsync(int userId);
    Task<(bool Success, string? ErrorMessage)> SoftDeleteAsync(int userId);
    Task<List<User>> GetFamilyMembersAsync(int ownerUserId);
    Task<List<MyApartmentViewModel>> GetMyApartmentsAsync(int userId);
    Task<ApartmentDetailViewModel?> GetApartmentWithDetailsAsync(int apartmentId, int userId);
    Task<List<User>> GetFamilyMembersForApartmentAsync(int apartmentId);
    Task<List<ApartmentResidentDto>> GetAllResidentsForApartmentAsync(int apartmentId);
}

public class UserUpsertRequest
{
    public int? UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? DateOfBirth { get; set; }
    public string? IdentityCardNumber { get; set; }
    public ResidentType? ResidentType { get; set; }
    public string? ResidencyStatus { get; set; }
    public int? ApartmentId { get; set; }
    public DateTime? MoveInDate { get; set; }
    public DateTime? MoveOutDate { get; set; }
    public string? Note { get; set; }
}
