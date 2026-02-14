using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Account;

[AllowAnonymous]
public class ForgotPasswordModel : PageModel
{
    private readonly IAuthService _authService;

    public ForgotPasswordModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    [Required(ErrorMessage = "Vui lòng nhập tài khoản hoặc email")]
    public string UsernameOrEmail { get; set; } = string.Empty;

    public string? Message { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var (success, errorMessage) = await _authService.ResetPasswordAsync(UsernameOrEmail);

        if (success)
        {
            Message = "Mật khẩu mới đã được gửi tới Email của bạn.";
        }
        else
        {
            ModelState.AddModelError(string.Empty, errorMessage);
        }

        return Page();
    }
}

