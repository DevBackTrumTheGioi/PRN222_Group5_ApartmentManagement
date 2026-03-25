using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
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

                // if (string.IsNullOrEmpty(returnUrl) || returnUrl == "/" || returnUrl == Url.Content("~/"))
                // {
                //     return user.Role switch
                //     {
                //         UserRole.Admin => RedirectToPage("/Admin/Index"),
                //         UserRole.BQL_Manager => RedirectToPage("/BQL_Manager/Index"),
                //         UserRole.BQL_Staff => RedirectToPage("/BQL_Staff/Index"),
                //         UserRole.Resident => RedirectToPage("/Resident/Index"),
                //         UserRole.BQT_Head => RedirectToPage("/BQT_Head/Index"),
                //         UserRole.BQT_Member => RedirectToPage("/BQT_Member/Index"),
                //         _ => LocalRedirect(returnUrl ?? "~/")
                //     };
                // }
                //
                // return LocalRedirect(returnUrl);

                var targetUrl = ResolvePostLoginUrl(returnUrl, user.Role);
                return LocalRedirect(targetUrl);
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

    private string ResolvePostLoginUrl(string? returnUrl, UserRole? role)
    {
        var defaultUrl = GetDefaultUrlByRole(role);

        if (string.IsNullOrWhiteSpace(returnUrl) || returnUrl == "/" || returnUrl == Url.Content("~/"))
        {
            return defaultUrl;
        }

        if (!Url.IsLocalUrl(returnUrl) || !IsReturnUrlAllowedByRole(returnUrl, role))
        {
            return defaultUrl;
        }

        return returnUrl;
    }

    private static string GetDefaultUrlByRole(UserRole? role)
    {
        return role switch
        {
            UserRole.Admin => "/Admin/Index",
            UserRole.BQL_Manager => "/BQL_Manager/Index",
            UserRole.BQL_Staff => "/BQL_Staff/Index",
            UserRole.Resident => "/Resident/Index",
            UserRole.BQT_Head => "/BQT_Head/Index",
            UserRole.BQT_Member => "/BQT_Member/Index",
            _ => "/"
        };
    }

    private static bool IsReturnUrlAllowedByRole(string returnUrl, UserRole? role)
    {
        var path = returnUrl.Split('?', '#')[0];

        if (path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase))
            return role == UserRole.Admin;

        if (path.StartsWith("/BQL_Manager", StringComparison.OrdinalIgnoreCase))
            return role == UserRole.BQL_Manager;

        if (path.StartsWith("/BQL_Staff", StringComparison.OrdinalIgnoreCase))
            return role == UserRole.BQL_Staff;

        if (path.StartsWith("/Resident", StringComparison.OrdinalIgnoreCase))
            return role == UserRole.Resident;

        if (path.StartsWith("/BQT_Head", StringComparison.OrdinalIgnoreCase))
            return role == UserRole.BQT_Head;

        if (path.StartsWith("/BQT_Member", StringComparison.OrdinalIgnoreCase))
            return role == UserRole.BQT_Member;

        if (path.StartsWith("/Announcements/Create", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/Announcements/Edit", StringComparison.OrdinalIgnoreCase))
        {
            return role is UserRole.BQL_Manager or UserRole.BQT_Head;
        }

        return true;
    }
}
