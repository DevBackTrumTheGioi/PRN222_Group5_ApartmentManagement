using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Repositories.Interfaces;

public interface IUserRefreshTokenRepository : IGenericRepository<UserRefreshToken>
{
    Task<UserRefreshToken?> GetByTokenHashAsync(string tokenHash);
    Task<List<UserRefreshToken>> GetActiveTokensByUserIdAsync(int userId);
}
