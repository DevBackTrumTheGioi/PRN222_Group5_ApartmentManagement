using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Services.Interfaces;
using PRN222_ApartmentManagement.Utils;
using BC = BCrypt.Net.BCrypt;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly ApartmentDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthService(ApartmentDbContext context, IConfiguration configuration, IEmailService emailService)
    {
        _context = context;
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task<(bool Success, string Token, User? User, string ErrorMessage)> LoginAsync(string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive && !u.IsDeleted);

        if (user == null || !VerifyPassword(password, user.PasswordHash))
        {
            return (false, string.Empty, null, "Invalid username or password.");
        }

        var token = GenerateJwtToken(user);
        
        user.LastLogin = DateTime.Now;
        await _context.SaveChangesAsync();

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
        var user = await _context.Users.FindAsync(userId);
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
        
        await _context.SaveChangesAsync();
        return (true, string.Empty);
    }

    public async Task<(bool Success, string ErrorMessage)> ResetPasswordAsync(string usernameOrEmail)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);

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
            
            await _context.SaveChangesAsync();
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, $"Lỗi khi gửi email: {ex.Message}");
        }
    }
}
