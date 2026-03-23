using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services;
using PRN222_ApartmentManagement.Services.Interfaces;
using PRN222_ApartmentManagement.Utils;
using AuthCookieHelper = PRN222_ApartmentManagement.Utils.AuthCookieHelper;

namespace PRN222_ApartmentManagement.Pages.Account;

[AllowAnonymous]
public class VerifyPhoneModel : PageModel
{
    private readonly ISmsService _smsService;
    private readonly IUserRepository _userRepository;
    private readonly IActivityLogService _activityLogService;
    private readonly IAuthService _authService;

    public const int ResendCooldownSeconds = 60;

    public VerifyPhoneModel(
        ISmsService smsService,
        IUserRepository userRepository,
        IActivityLogService activityLogService,
        IAuthService authService)
    {
        _smsService = smsService;
        _userRepository = userRepository;
        _activityLogService = activityLogService;
        _authService = authService;
    }

    [BindProperty]
    public string? PhoneNumber { get; set; }

    [BindProperty]
    public string? OtpCode { get; set; }

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsVerified { get; set; }

    /// <summary>OTP đã được gửi thành công — hiện form nhập OTP</summary>
    public bool OtpSent { get; set; }

    /// <summary>Có đang ở chế độ đổi số điện thoại không</summary>
    public bool ChangingPhone { get; set; }

    /// <summary>Thời điểm OTP được gửi (Unix seconds) cho JS countdown</summary>
    public long SentAt { get; set; }

    /// <summary>Số điện thoại gốc từ DB</summary>
    public string? OriginalPhone { get; set; }

    public async Task<IActionResult> OnGet()
    {
        // Sau khi xác minh thành công: PRG — hiển thị màn hình thành công (không cần session)
        if (TempData["PhoneVerifySuccess"] is bool success && success)
        {
            IsVerified = true;
            return Page();
        }

        var pendingUserId = HttpContext.Session.GetInt32("PendingUserId");
        if (!pendingUserId.HasValue)
            return RedirectToPage("/Account/Login");

        var user = await _userRepository.GetActiveByIdAsync(pendingUserId.Value);
        if (user == null)
        {
            HttpContext.Session.Remove("PendingUserId");
            return RedirectToPage("/Account/Login");
        }

        if (user.IsActive)
            return RedirectToPage("/Resident/Index");

        PhoneNumber = user.PhoneNumber;
        OriginalPhone = user.PhoneNumber;
        HttpContext.Session.SetInt32("PendingUserId", user.UserId);

        RestoreState();
        return Page();
    }

    /// <summary>
    /// Bước 1a: Gửi OTP đến số hiện tại trong DB
    /// </summary>
    public async Task<IActionResult> OnPostSendOtp()
    {
        var pendingUserId = HttpContext.Session.GetInt32("PendingUserId");
        if (!pendingUserId.HasValue)
            return RedirectToPage("/Account/Login");

        var user = await _userRepository.GetActiveByIdAsync(pendingUserId.Value);
        if (user == null)
        {
            HttpContext.Session.Remove("PendingUserId");
            return RedirectToPage("/Account/Login");
        }

        if (user.IsActive)
            return RedirectToPage("/Resident/Index");

        OriginalPhone = user.PhoneNumber;
        PhoneNumber = user.PhoneNumber;

        return await SendOtpToPhone(user.PhoneNumber ?? "", user.UserId);
    }

    /// <summary>
    /// Bước 1b: Chuyển sang chế độ nhập số mới
    /// </summary>
    public async Task<IActionResult> OnPostChangePhone()
    {
        var pendingUserId = HttpContext.Session.GetInt32("PendingUserId");
        if (!pendingUserId.HasValue)
            return RedirectToPage("/Account/Login");

        var user = await _userRepository.GetActiveByIdAsync(pendingUserId.Value);
        if (user == null)
        {
            HttpContext.Session.Remove("PendingUserId");
            return RedirectToPage("/Account/Login");
        }

        OriginalPhone = user.PhoneNumber;
        ChangingPhone = true;
        return Page();
    }

    /// <summary>
    /// Bước 1c: Validate số mới — gửi OTP (CHƯA LƯU DB)
    /// </summary>
    public async Task<IActionResult> OnPostVerifyNewPhone()
    {
        var pendingUserId = HttpContext.Session.GetInt32("PendingUserId");
        if (!pendingUserId.HasValue)
            return RedirectToPage("/Account/Login");

        var user = await _userRepository.GetActiveByIdAsync(pendingUserId.Value);
        if (user == null)
        {
            HttpContext.Session.Remove("PendingUserId");
            return RedirectToPage("/Account/Login");
        }

        OriginalPhone = user.PhoneNumber;

        if (string.IsNullOrWhiteSpace(PhoneNumber))
        {
            ErrorMessage = "Số điện thoại không được để trống.";
            HttpContext.Session.SetString("IsChangingPhone", "1");
            ChangingPhone = true;
            return Page();
        }

        var normalized = StringUtils.NormalizePhoneNumber(PhoneNumber);
        if (normalized.Length < 9 || normalized.Length > 15)
        {
            ErrorMessage = "Số điện thoại không hợp lệ.";
            HttpContext.Session.SetString("IsChangingPhone", "1");
            ChangingPhone = true;
            return Page();
        }

        if (normalized == user.PhoneNumber)
        {
            ErrorMessage = "Số này trùng với số hiện tại. Không cần thay đổi.";
            HttpContext.Session.SetString("IsChangingPhone", "1");
            ChangingPhone = true;
            return Page();
        }

        if (await _userRepository.PhoneExistsAsync(normalized, user.UserId))
        {
            ErrorMessage = $"Số điện thoại \"{normalized}\" đã được sử dụng bởi tài khoản khác.";
            HttpContext.Session.SetString("IsChangingPhone", "1");
            ChangingPhone = true;
            return Page();
        }

        // CHƯA lưu — chỉ gửi OTP để verify
        HttpContext.Session.SetString("IsChangingPhone", "1");
        return await SendOtpToPhone(normalized, user.UserId);
    }

    /// <summary>
    /// Bước 2: Verify OTP — nếu đổi số thì LƯU vào DB, rồi activate
    /// </summary>
    public async Task<IActionResult> OnPostVerify()
    {
        var pendingUserId = HttpContext.Session.GetInt32("PendingUserId");
        if (!pendingUserId.HasValue)
            return RedirectToPage("/Account/Login");

        var user = await _userRepository.GetActiveByIdAsync(pendingUserId.Value);
        if (user == null)
        {
            HttpContext.Session.Remove("PendingUserId");
            return RedirectToPage("/Account/Login");
        }

        RestoreState();
        OriginalPhone = user.PhoneNumber;

        if (string.IsNullOrWhiteSpace(OtpCode))
        {
            ErrorMessage = "Vui lòng nhập mã OTP.";
            return Page();
        }

        if (OtpCode.Length != 6 || !OtpCode.All(char.IsDigit))
        {
            ErrorMessage = "Mã OTP phải là 6 chữ số.";
            return Page();
        }

        // Số điện thoại để verify = số đang chờ trong session (có thể là số mới nếu đổi)
        var phoneToVerify = HttpContext.Session.GetString("PendingPhone") ?? user.PhoneNumber ?? "";

        var (success, error) = await _smsService.VerifyOtpAsync(phoneToVerify, OtpCode, OtpIntent.AccountActivation);

        if (success)
        {
            // Nếu đang đổi số → LƯU vào DB ngay khi verify thành công
            if (ChangingPhone && !string.IsNullOrWhiteSpace(phoneToVerify) && phoneToVerify != user.PhoneNumber)
            {
                var oldPhone = user.PhoneNumber;
                user.PhoneNumber = phoneToVerify;
                user.UpdatedAt = DateTime.Now;
                await _userRepository.UpdateAsync(user);

                await _activityLogService.LogCustomAsync(
                    "PhoneNumberChanged",
                    nameof(User),
                    user.UserId.ToString(),
                    $"Đổi số điện thoại từ {oldPhone} thành {phoneToVerify}.");

                OriginalPhone = phoneToVerify;
                PhoneNumber = phoneToVerify;
            }

            user.IsActive = true;
            user.UpdatedAt = DateTime.Now;
            await _userRepository.UpdateAsync(user);

            await _activityLogService.LogCustomAsync(
                "AccountActivated",
                nameof(User),
                user.UserId.ToString(),
                $"Tài khoản {user.Username} đã được kích hoạt.");

            HttpContext.Session.Remove("PendingUserId");
            HttpContext.Session.Remove("OtpSentAt");
            HttpContext.Session.Remove("PendingPhone");
            HttpContext.Session.Remove("IsChangingPhone");

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var tokens = await _authService.GenerateTokensForUserAsync(user.UserId, ip);
            if (tokens != null)
                AuthCookieHelper.SetAuthCookies(HttpContext, tokens);

            TempData["PhoneVerifySuccess"] = true;
            return RedirectToPage("/Account/VerifyPhone");
        }

        ErrorMessage = string.IsNullOrWhiteSpace(error)
            ? "Mã OTP không đúng hoặc đã hết hạn."
            : error;
        return Page();
    }

    /// <summary>
    /// Gửi lại OTP — chỉ khi đã hết cooldown
    /// </summary>
    public async Task<IActionResult> OnPostResend()
    {
        var pendingUserId = HttpContext.Session.GetInt32("PendingUserId");
        if (!pendingUserId.HasValue)
            return RedirectToPage("/Account/Login");

        var user = await _userRepository.GetActiveByIdAsync(pendingUserId.Value);
        if (user == null)
        {
            HttpContext.Session.Remove("PendingUserId");
            return RedirectToPage("/Account/Login");
        }

        RestoreState();
        OriginalPhone = user.PhoneNumber;

        if (HttpContext.Session.TryGetValue("OtpSentAt", out var sentAtBytes))
        {
            var lastSent = long.Parse(System.Text.Encoding.UTF8.GetString(sentAtBytes));
            var elapsed = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - lastSent;
            if (elapsed < ResendCooldownSeconds)
            {
                ErrorMessage = $"Vui lòng chờ {ResendCooldownSeconds - elapsed} giây trước khi gửi lại.";
                return Page();
            }
        }

        // Gửi lại đến số đang chờ (số mới nếu đổi, không thì số gốc)
        var phoneToResend = HttpContext.Session.GetString("PendingPhone") ?? user.PhoneNumber ?? "";
        return await SendOtpToPhone(phoneToResend, user.UserId);
    }

    private async Task<IActionResult> SendOtpToPhone(string phone, int userId)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            ErrorMessage = "Số điện thoại không hợp lệ.";
            OriginalPhone = phone;
            ChangingPhone = true;
            return Page();
        }

        var (success, error) = await _smsService.SendOtpAsync(phone, OtpIntent.AccountActivation);

        if (success)
        {
            SentAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            HttpContext.Session.SetString("OtpSentAt", SentAt.ToString());
            HttpContext.Session.SetString("PendingPhone", phone);
            OtpSent = true;
            SuccessMessage = $"Đã gửi mã OTP đến {phone}. Vui lòng kiểm tra tin nhắn.";
        }
        else
        {
            ErrorMessage = $"Lỗi gửi OTP: {error}";
            ChangingPhone = true;
        }

        OriginalPhone = phone;
        PhoneNumber = phone;
        return Page();
    }

    private void RestoreState()
    {
        OtpSent = false;
        ChangingPhone = HttpContext.Session.GetString("IsChangingPhone") == "1";
        if (HttpContext.Session.TryGetValue("OtpSentAt", out var sentAtBytes))
        {
            SentAt = long.Parse(System.Text.Encoding.UTF8.GetString(sentAtBytes));
            if (SentAt > 0)
                OtpSent = true;
        }
    }
}
