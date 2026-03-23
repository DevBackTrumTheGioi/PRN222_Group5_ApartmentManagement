using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;
using PRN222_ApartmentManagement.Utils;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class SmsService : ISmsService
{
    private readonly UniMatrixHelper _uniMatrixHelper;

    public SmsService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _uniMatrixHelper = new UniMatrixHelper(httpClientFactory, configuration);
    }

    public async Task<(bool Success, string Error)> SendCredentialsAsync(string phone, string username, string password)
    {
        var message = $"[Chung cu] Tai khoan cua ban:\n\nUsername: {username}\nPassword: {password}\n\n" +
                      $"Vui long dang nhap va xac minh so dien thoai de kich hoat tai khoan.";

        var result = await _uniMatrixHelper.SendSmsAsync(phone, message);
        return (result.Success, result.Error ?? "");
    }

    public async Task<(bool Success, string Error)> SendSmsAsync(string phone, string message)
    {
        var result = await _uniMatrixHelper.SendSmsAsync(phone, message);
        return (result.Success, result.Error ?? "");
    }

    public async Task<(bool Success, string Error)> SendOtpAsync(string phone, OtpIntent intent)
    {
        var result = await _uniMatrixHelper.SendOtpAsync(phone, intent);
        return (result.Success, result.Error ?? "");
    }

    public async Task<(bool Success, string Error)> VerifyOtpAsync(string phone, string code, OtpIntent intent)
    {
        var result = await _uniMatrixHelper.VerifyOtpAsync(phone, code, intent);
        if (result.Valid)
            return (true, "");
        return (false, result.Error ?? "OTP khong hop le.");
    }
}
