using Microsoft.AspNetCore.Http;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Utils;

public static class AuthCookieHelper
{
    public const string AccessTokenCookieName = "AccessToken";
    public const string RefreshTokenCookieName = "RefreshToken";

    public static void SetAuthCookies(HttpContext httpContext, TokenPairResult tokens)
    {
        httpContext.Response.Cookies.Append(
            AccessTokenCookieName,
            tokens.AccessToken,
            CreateCookieOptions(httpContext, tokens.AccessTokenExpiresAt));

        httpContext.Response.Cookies.Append(
            RefreshTokenCookieName,
            tokens.RefreshToken,
            CreateCookieOptions(httpContext, tokens.RefreshTokenExpiresAt));
    }

    public static void ClearAuthCookies(HttpContext httpContext)
    {
        var options = CreateCookieOptions(httpContext, DateTime.UtcNow.AddDays(-1));
        httpContext.Response.Cookies.Delete(AccessTokenCookieName, options);
        httpContext.Response.Cookies.Delete(RefreshTokenCookieName, options);
    }

    private static CookieOptions CreateCookieOptions(HttpContext httpContext, DateTime expiresAtUtc)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = httpContext.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = new DateTimeOffset(expiresAtUtc)
        };
    }
}
