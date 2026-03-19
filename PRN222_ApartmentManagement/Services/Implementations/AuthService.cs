using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
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
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly IActivityLogService _activityLogService;

    public AuthService(
        IUserRepository userRepository,
        IConfiguration configuration,
        IEmailService emailService,
        IActivityLogService activityLogService)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _emailService = emailService;
        _activityLogService = activityLogService;
    }

    public async Task<(bool Success, string Token, User? User, string ErrorMessage)> LoginAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);

        if (user == null || !user.IsActive || !VerifyPassword(password, user.PasswordHash))
        {
            await _activityLogService.LogLoginAsync(0, username, false, "Invalid username or password.");
            return (false, string.Empty, null, "Invalid username or password.");
        }

        var token = GenerateJwtToken(user);
        
        user.LastLogin = DateTime.Now;
        await _userRepository.UpdateAsync(user);
        await _activityLogService.LogLoginAsync(user.UserId, user.Username, true);

        return (true, token, user, string.Empty);
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

    private string GenerateJwtToken(User user)
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
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["DurationInMinutes"]!)),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
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
}
