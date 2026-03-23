using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Services.Interfaces;

public interface ISmsService
{
    Task<(bool Success, string Error)> SendCredentialsAsync(string phone, string username, string password);
    Task<(bool Success, string Error)> SendSmsAsync(string phone, string message);
    Task<(bool Success, string Error)> SendOtpAsync(string phone, OtpIntent intent);
    Task<(bool Success, string Error)> VerifyOtpAsync(string phone, string code, OtpIntent intent);
}
