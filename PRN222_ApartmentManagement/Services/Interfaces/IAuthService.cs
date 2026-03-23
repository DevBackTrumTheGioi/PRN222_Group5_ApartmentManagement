using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Services.Interfaces;

public interface IAuthService
{
    Task<(bool Success, TokenPairResult? Tokens, User? User, string ErrorMessage, bool IsInactiveAccount)> LoginAsync(string username, string password, string? ipAddress = null);
    Task<(bool Success, TokenPairResult? Tokens, User? User, string ErrorMessage)> RefreshTokenAsync(string refreshToken, string? ipAddress = null);
    Task<(bool Success, string ErrorMessage)> RevokeRefreshTokenAsync(string refreshToken, string? ipAddress = null, string? reason = null);
    Task RevokeAllRefreshTokensAsync(int userId, string? ipAddress = null, string? reason = null);
    Task<(bool Success, string ErrorMessage)> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    Task<(bool Success, string ErrorMessage)> ResetPasswordAsync(string usernameOrEmail);
    Task LogLogoutAsync(int userId, string userName);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
    Task<TokenPairResult?> GenerateTokensForUserAsync(int userId, string? ipAddress = null);
}

public class TokenPairResult
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
}
