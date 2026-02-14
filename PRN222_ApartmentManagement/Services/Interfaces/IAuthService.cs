using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Services.Interfaces;

public interface IAuthService
{
    Task<(bool Success, string Token, User? User, string ErrorMessage)> LoginAsync(string username, string password);
    Task<(bool Success, string ErrorMessage)> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    Task<(bool Success, string ErrorMessage)> ResetPasswordAsync(string usernameOrEmail);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}
