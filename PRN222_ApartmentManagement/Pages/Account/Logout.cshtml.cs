using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Services.Interfaces;
using PRN222_ApartmentManagement.Utils;

namespace PRN222_ApartmentManagement.Pages.Account;

public class LogoutModel : PageModel
{
    private readonly IAuthService _authService;

    public LogoutModel(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var userName = User.Identity?.Name;
        var refreshToken = Request.Cookies[AuthCookieHelper.RefreshTokenCookieName];
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            await _authService.RevokeRefreshTokenAsync(refreshToken, ipAddress, "User logout.");
        }

        if (int.TryParse(userIdClaim, out var userId) && !string.IsNullOrWhiteSpace(userName))
        {
            await _authService.LogLogoutAsync(userId, userName);
        }

        AuthCookieHelper.ClearAuthCookies(HttpContext);
        return RedirectToPage("/Account/Login");
    }
}
