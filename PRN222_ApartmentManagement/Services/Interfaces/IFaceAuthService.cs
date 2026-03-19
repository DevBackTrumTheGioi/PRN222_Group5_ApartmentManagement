using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Services.Interfaces;

public interface IFaceAuthService
{
    Task<(bool Success, string? ErrorMessage)> RegisterFaceAsync(int userId, string faceDescriptor);
    Task<User?> GetResidentByIdAsync(int userId);
    Task<List<FaceAuthHistory>> GetRecentHistoriesAsync(int userId, int take);
    Task<List<FaceAuthHistory>> GetHistoriesAsync(int userId);
}
