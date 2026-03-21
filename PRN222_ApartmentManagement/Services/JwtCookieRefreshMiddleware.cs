using Microsoft.IdentityModel.Tokens;
using PRN222_ApartmentManagement.Services.Interfaces;
using PRN222_ApartmentManagement.Utils;
using System.IdentityModel.Tokens.Jwt;

namespace PRN222_ApartmentManagement.Services;

public class JwtCookieRefreshMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public JwtCookieRefreshMiddleware(RequestDelegate next, TokenValidationParameters tokenValidationParameters)
    {
        _next = next;
        _tokenValidationParameters = tokenValidationParameters;
    }

    public async Task InvokeAsync(HttpContext context, IAuthService authService)
    {
        if (HasBearerHeader(context.Request))
        {
            await _next(context);
            return;
        }

        var accessToken = context.Request.Cookies[AuthCookieHelper.AccessTokenCookieName];
        var refreshToken = context.Request.Cookies[AuthCookieHelper.RefreshTokenCookieName];

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            await TryRefreshAsync(context, authService, refreshToken);
            await _next(context);
            return;
        }

        if (IsAccessTokenValid(accessToken))
        {
            await _next(context);
            return;
        }

        await TryRefreshAsync(context, authService, refreshToken);
        await _next(context);
    }

    private bool IsAccessTokenValid(string accessToken)
    {
        try
        {
            _tokenHandler.ValidateToken(accessToken, _tokenValidationParameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task TryRefreshAsync(HttpContext context, IAuthService authService, string? refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return;
        }

        var (success, tokens, _, _) = await authService.RefreshTokenAsync(refreshToken, context.Connection.RemoteIpAddress?.ToString());

        if (!success || tokens == null)
        {
            AuthCookieHelper.ClearAuthCookies(context);
            return;
        }

        context.Items[AuthCookieHelper.AccessTokenCookieName] = tokens.AccessToken;
        AuthCookieHelper.SetAuthCookies(context, tokens);
    }

    private static bool HasBearerHeader(HttpRequest request)
    {
        var authHeader = request.Headers["Authorization"].ToString();
        return !string.IsNullOrWhiteSpace(authHeader) &&
               authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase);
    }
}
