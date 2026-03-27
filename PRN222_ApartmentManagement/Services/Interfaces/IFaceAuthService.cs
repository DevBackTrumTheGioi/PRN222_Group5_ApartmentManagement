using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;

namespace PRN222_ApartmentManagement.Services.Interfaces;

public interface IFaceAuthService
{
    Task<(bool Success, string? ErrorMessage)> RegisterFaceAsync(int userId, string faceDescriptor);
    Task<(bool Success, string? ErrorMessage)> RegisterFaceForResidentAsync(int actorUserId, int residentId, string faceDescriptor);
    Task<(bool Success, string? ErrorMessage)> ResetFaceAsync(int actorUserId, int residentId);
    Task<User?> GetResidentByIdAsync(int userId);
    Task<User?> GetResidentForManagementAsync(int userId);
    Task<List<FaceAuthHistory>> GetRecentHistoriesAsync(int userId, int take);
    Task<List<FaceAuthHistory>> GetHistoriesAsync(int userId);
    Task<List<FaceResidentSummaryDto>> GetResidentSummariesAsync(string? searchTerm, bool? isRegistered);
    Task<List<FaceAuthLogDto>> GetManagementLogsAsync(string? searchTerm, bool? isSuccess, DateTime? fromDate, DateTime? toDate, int take = 200);
    Task<FaceAuthDashboardDto> GetDashboardAsync(int recentDays = 30);
    Task<AmenityFaceAccessResultDto> ValidateAmenityAccessAsync(int amenityId, string faceDescriptor, string? ipAddress, string? deviceInfo);
    Task<AmenityFaceAccessResultDto> ValidateAmenityAccessManualAsync(int amenityId, int residentId, int staffUserId, string? ipAddress, string? deviceInfo);
}
