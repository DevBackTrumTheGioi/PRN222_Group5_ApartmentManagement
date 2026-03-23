using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Services.Interfaces;
using PRN222_ApartmentManagement.Utils;

namespace PRN222_ApartmentManagement.Pages.Account;

public class LoginModel : PageModel
{
    private readonly IAuthService _authService;

    public LoginModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (ModelState.IsValid)
        {
            var (success, tokens, user, errorMessage, isInactiveAccount) = await _authService.LoginAsync(
                Input.Username,
                Input.Password,
                HttpContext.Connection.RemoteIpAddress?.ToString());

            if (success && user != null && tokens != null)
            {
                AuthCookieHelper.SetAuthCookies(HttpContext, tokens);

                if (string.IsNullOrEmpty(returnUrl) || returnUrl == "/" || returnUrl == Url.Content("~/"))
                {
                    return user.Role switch
                    {
                        Models.UserRole.Admin => RedirectToPage("/Admin/Index"),
                        Models.UserRole.BQL_Manager => RedirectToPage("/BQL_Manager/Index"),
                        Models.UserRole.BQL_Staff => RedirectToPage("/BQL_Staff/Index"),
                        Models.UserRole.Resident => RedirectToPage("/Resident/Index"),
                        Models.UserRole.BQT_Head => RedirectToPage("/BQT_Head/Index"),
                        Models.UserRole.BQT_Member => RedirectToPage("/BQT_Member/Index"),
                        _ => LocalRedirect(returnUrl ?? "~/")
                    };
                }

                return LocalRedirect(returnUrl);
            }

            if (isInactiveAccount)
            {
                HttpContext.Session.SetInt32("PendingUserId", user.UserId);
                HttpContext.Session.SetString("PendingUsername", user.Username ?? Input.Username);
                return RedirectToPage("/Account/VerifyPhone");
            }

            ModelState.AddModelError(string.Empty, errorMessage);
        }

        return Page();
    }
}
