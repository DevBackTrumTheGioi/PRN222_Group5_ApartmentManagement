using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager;

[Authorize(Roles = "BQL_Manager")]
public class TestSmsModel : PageModel
{
    private readonly ISmsService _smsService;

    public TestSmsModel(ISmsService smsService)
    {
        _smsService = smsService;
    }

    [BindProperty]
    public string PhoneNumber { get; set; } = string.Empty;

    [BindProperty]
    public string OtpCode { get; set; } = string.Empty;

    [BindProperty]
    public OtpIntent OtpIntent { get; set; } = OtpIntent.AccountActivation;

    public bool OtpSent { get; set; }
    public bool IsVerified { get; set; }
    public string? Message { get; set; }

    public void OnGet()
    {
        PhoneNumber = string.Empty;
    }

    public async Task<IActionResult> OnPostSendOtpAsync()
    {
        if (string.IsNullOrWhiteSpace(PhoneNumber))
        {
            Message = "Số điện thoại không được để trống.";
            return Page();
        }

        var (success, error) = await _smsService.SendOtpAsync(PhoneNumber, OtpIntent);

        if (success)
        {
            OtpSent = true;
            Message = $"Đã gửi mã OTP đến {PhoneNumber}. Vui lòng kiểm tra tin nhắn.";
        }
        else
        {
            Message = $"Lỗi gửi OTP: {error}";
        }

        return Page();
    }

    public async Task<IActionResult> OnPostVerifyOtpAsync()
    {
        if (string.IsNullOrWhiteSpace(PhoneNumber))
        {
            Message = "Số điện thoại không được để trống.";
            OtpSent = true;
            return Page();
        }

        if (string.IsNullOrWhiteSpace(OtpCode))
        {
            Message = "Vui lòng nhập mã OTP.";
            OtpSent = true;
            return Page();
        }

        if (OtpCode.Length != 6 || !OtpCode.All(char.IsDigit))
        {
            Message = "Mã OTP phải là 6 chữ số.";
            OtpSent = true;
            return Page();
        }

        var (success, error) = await _smsService.VerifyOtpAsync(PhoneNumber, OtpCode, OtpIntent);

        if (success)
        {
            IsVerified = true;
            Message = "OTP hợp lệ! Xác minh thành công.";
        }
        else
        {
            OtpSent = true;
            Message = $"OTP không hợp lệ hoặc đã hết hạn: {error}";
        }

        return Page();
    }
}
