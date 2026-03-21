using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class FaceAuthService : IFaceAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IGenericRepository<FaceAuthHistory> _faceAuthHistoryRepository;

    public FaceAuthService(
        IUserRepository userRepository,
        IGenericRepository<FaceAuthHistory> faceAuthHistoryRepository)
    {
        _userRepository = userRepository;
        _faceAuthHistoryRepository = faceAuthHistoryRepository;
    }

    public async Task<(bool Success, string? ErrorMessage)> RegisterFaceAsync(int userId, string faceDescriptor)
    {
        var user = await GetResidentByIdAsync(userId);
        if (user == null)
        {
            return (false, "Không tìm thấy cư dân.");
        }

        user.FaceDescriptor = faceDescriptor;
        user.IsFaceRegistered = true;
        user.UpdatedAt = DateTime.Now;

        await _userRepository.UpdateAsync(user);
        return (true, null);
    }

    public async Task<User?> GetResidentByIdAsync(int userId)
    {
        var user = await _userRepository.GetActiveByIdAsync(userId);
        if (user == null || user.Role != UserRole.Resident)
        {
            return null;
        }

        return user;
    }

    public async Task<List<FaceAuthHistory>> GetRecentHistoriesAsync(int userId, int take)
    {
        var histories = await _faceAuthHistoryRepository.FindAsync(h => h.ResidentId == userId);
        return histories.OrderByDescending(h => h.AuthTime).Take(take).ToList();
    }

    public async Task<List<FaceAuthHistory>> GetHistoriesAsync(int userId)
    {
        var histories = await _faceAuthHistoryRepository.FindAsync(h => h.ResidentId == userId);
        return histories.OrderByDescending(h => h.AuthTime).ToList();
    }
}
