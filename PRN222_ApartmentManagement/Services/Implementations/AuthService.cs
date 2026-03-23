using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services.Interfaces;
using PRN222_ApartmentManagement.Utils;
using BC = BCrypt.Net.BCrypt;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRefreshTokenRepository _userRefreshTokenRepository;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly IActivityLogService _activityLogService;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public AuthService(
        IUserRepository userRepository,
        IUserRefreshTokenRepository userRefreshTokenRepository,
        IConfiguration configuration,
        IEmailService emailService,
        IActivityLogService activityLogService)
    {
        _userRepository = userRepository;
        _userRefreshTokenRepository = userRefreshTokenRepository;
        _configuration = configuration;
        _emailService = emailService;
        _activityLogService = activityLogService;
    }

    public async Task<(bool Success, TokenPairResult? Tokens, User? User, string ErrorMessage, bool IsInactiveAccount)> LoginAsync(
        string username,
        string password,
        string? ipAddress = null)
    {
        var user = await _userRepository.GetByUsernameAsync(username);

        if (user == null || user.IsDeleted || !VerifyPassword(password, user.PasswordHash))
        {
            await _activityLogService.LogLoginAsync(0, username, false, "Invalid username or password.");
            return (false, null, null, "Invalid username or password.", false);
        }

        if (!user.IsActive)
        {
            await _activityLogService.LogLoginAsync(user.UserId, username, false, "Account is inactive.");
            return (false, null, user, "Account is inactive.", true);
        }

        var tokens = await GenerateTokenPairAsync(user, ipAddress);

        user.LastLogin = DateTime.Now;
        user.UpdatedAt = DateTime.Now;
        await _userRepository.UpdateAsync(user);
        await _activityLogService.LogLoginAsync(user.UserId, user.Username, true);

        return (true, tokens, user, string.Empty, false);
    }

    public async Task<(bool Success, TokenPairResult? Tokens, User? User, string ErrorMessage)> RefreshTokenAsync(
        string refreshToken,
        string? ipAddress = null)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return (false, null, null, "Refresh token is required.");
        }

        var storedToken = await _userRefreshTokenRepository.GetByTokenHashAsync(HashRefreshToken(refreshToken));

        if (storedToken == null || storedToken.User == null)
        {
            await _activityLogService.LogCustomAsync(
                "RefreshTokenFailed",
                nameof(UserRefreshToken),
                description: "Refresh token not found.");
            return (false, null, null, "Refresh token is invalid.");
        }

        if (storedToken.RevokedAt.HasValue)
        {
            await RevokeAllRefreshTokensAsync(storedToken.UserId, ipAddress, "Detected refresh token reuse.");
            await _activityLogService.LogCustomAsync(
                "RefreshTokenFailed",
                nameof(UserRefreshToken),
                storedToken.RefreshTokenId.ToString(),
                $"Refresh token reuse detected for user {storedToken.User.Username}");
            return (false, null, storedToken.User, "Refresh token is invalid.");
        }

        if (storedToken.ExpiresAt <= DateTime.Now || !storedToken.User.IsActive || storedToken.User.IsDeleted)
        {
            await _activityLogService.LogCustomAsync(
                "RefreshTokenFailed",
                nameof(UserRefreshToken),
                storedToken.RefreshTokenId.ToString(),
                $"Refresh token expired or user inactive for {storedToken.User.Username}");
            return (false, null, storedToken.User, "Refresh token is invalid or expired.");
        }

        var newTokens = await GenerateTokenPairAsync(storedToken.User, ipAddress);
        storedToken.RevokedAt = DateTime.Now;
        storedToken.ReplacedByTokenHash = HashRefreshToken(newTokens.RefreshToken);
        storedToken.RevokedByIp = ipAddress;
        storedToken.RevocationReason = "Rotated on refresh.";

        await _userRefreshTokenRepository.UpdateAsync(storedToken);
        await _activityLogService.LogCustomAsync(
            "RefreshTokenSuccess",
            nameof(UserRefreshToken),
            storedToken.RefreshTokenId.ToString(),
            $"Refresh token rotated for user {storedToken.User.Username}");

        return (true, newTokens, storedToken.User, string.Empty);
    }

    public async Task<(bool Success, string ErrorMessage)> RevokeRefreshTokenAsync(
        string refreshToken,
        string? ipAddress = null,
        string? reason = null)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return (false, "Refresh token is required.");
        }

        var storedToken = await _userRefreshTokenRepository.GetByTokenHashAsync(HashRefreshToken(refreshToken));
        if (storedToken == null)
        {
            return (false, "Refresh token not found.");
        }

        if (!storedToken.RevokedAt.HasValue)
        {
            storedToken.RevokedAt = DateTime.Now;
            storedToken.RevokedByIp = ipAddress;
            storedToken.RevocationReason = reason ?? "Manual revoke.";

            await _userRefreshTokenRepository.UpdateAsync(storedToken);
            await _activityLogService.LogCustomAsync(
                "RefreshTokenRevoked",
                nameof(UserRefreshToken),
                storedToken.RefreshTokenId.ToString(),
                $"Refresh token revoked for user {storedToken.User.Username}");
        }

        return (true, string.Empty);
    }

    public async Task RevokeAllRefreshTokensAsync(int userId, string? ipAddress = null, string? reason = null)
    {
        var activeTokens = await _userRefreshTokenRepository.GetActiveTokensByUserIdAsync(userId);

        foreach (var token in activeTokens)
        {
            token.RevokedAt = DateTime.Now;
            token.RevokedByIp = ipAddress;
            token.RevocationReason = reason ?? "Revoked in bulk.";
            await _userRefreshTokenRepository.UpdateAsync(token);
        }
    }

    public string HashPassword(string password)
    {
        return BC.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            return BC.Verify(password, hashedPassword);
        }
        catch
        {
            return false;
        }
    }

    private string GenerateJwtToken(User user, DateTime expiresAtUtc)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role?.ToString() ?? string.Empty),
            new Claim("FullName", user.FullName)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAtUtc,
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }

    private async Task<TokenPairResult> GenerateTokenPairAsync(User user, string? ipAddress)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var accessExpiresAtUtc = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["AccessTokenDurationMinutes"] ?? "15"));
        var refreshExpiresAtUtc = DateTime.UtcNow.AddDays(double.Parse(jwtSettings["RefreshTokenDurationDays"] ?? "7"));
        var refreshToken = GenerateRefreshToken();

        await _userRefreshTokenRepository.AddAsync(new UserRefreshToken
        {
            UserId = user.UserId,
            TokenHash = HashRefreshToken(refreshToken),
            ExpiresAt = refreshExpiresAtUtc.ToLocalTime(),
            CreatedAt = DateTime.Now,
            CreatedByIp = ipAddress
        });

        return new TokenPairResult
        {
            AccessToken = GenerateJwtToken(user, accessExpiresAtUtc),
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = accessExpiresAtUtc,
            RefreshTokenExpiresAt = refreshExpiresAtUtc
        };
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    private static string HashRefreshToken(string refreshToken)
    {
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
    }

    public async Task<(bool Success, string ErrorMessage)> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        var user = await _userRepository.GetActiveByIdAsync(userId);
        if (user == null)
        {
            return (false, "User not found.");
        }

        if (!VerifyPassword(oldPassword, user.PasswordHash))
        {
            return (false, "Mật khẩu cũ không chính xác.");
        }

        user.PasswordHash = HashPassword(newPassword);
        user.UpdatedAt = DateTime.Now;

        await _userRepository.UpdateAsync(user);
        await RevokeAllRefreshTokensAsync(user.UserId, reason: "Password changed.");
        await _activityLogService.LogCustomAsync("ChangePassword", nameof(User), user.UserId.ToString(), $"Người dùng {user.Username} đổi mật khẩu");
        return (true, string.Empty);
    }

    public async Task<(bool Success, string ErrorMessage)> ResetPasswordAsync(string usernameOrEmail)
    {
        var users = await _userRepository.FindAsync(u =>
            !u.IsDeleted && (u.Username == usernameOrEmail || u.Email == usernameOrEmail));
        var user = users.FirstOrDefault();

        if (user == null)
        {
            return (false, "Không tìm thấy người dùng với thông tin cung cấp.");
        }

        if (string.IsNullOrEmpty(user.Email))
        {
            return (false, "Người dùng này không có địa chỉ email để nhận mật khẩu mới.");
        }

        string newPassword = PasswordGenerator.GenerateRandomPassword(10);
        user.PasswordHash = HashPassword(newPassword);
        user.UpdatedAt = DateTime.Now;

        try
        {
            string emailBody = $@"
                <h2>Cấp lại mật khẩu wifi</h2>
                <p>Chào {user.FullName},</p>
                <p>Yêu cầu đặt lại mật khẩu của bạn đã được thực hiện.</p>
                <p><strong>Tên đăng nhập:</strong> {user.Username}</p>
                <p><strong>Mật khẩu mới:</strong> {newPassword}</p>
                <p>Vui lòng đổi mật khẩu ngay sau khi đăng nhập thành công.</p>
                <br/>
                <p>Trân trọng,<br/>Ban quản lý chung cư</p>";

            await _emailService.SendEmailAsync(user.Email, "Cấp lại mật khẩu truy cập", emailBody);

            await _userRepository.UpdateAsync(user);
            await RevokeAllRefreshTokensAsync(user.UserId, reason: "Password reset.");
            await _activityLogService.LogCustomAsync("ResetPassword", nameof(User), user.UserId.ToString(), $"Đặt lại mật khẩu cho {user.Username}");
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, $"Lỗi khi gửi email: {ex.Message}");
        }
    }

    public async Task LogLogoutAsync(int userId, string userName)
    {
        await _activityLogService.LogLogoutAsync(userId, userName);
    }

    public async Task<TokenPairResult?> GenerateTokensForUserAsync(int userId, string? ipAddress = null)
    {
        var user = await _userRepository.GetActiveByIdAsync(userId);
        if (user == null || user.IsDeleted)
            return null;
        return await GenerateTokenPairAsync(user, ipAddress);
    }
}
