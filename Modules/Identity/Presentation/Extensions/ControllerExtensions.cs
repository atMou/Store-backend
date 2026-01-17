using Microsoft.AspNetCore.Mvc;

namespace Identity.Presentation.Extensions;

public static class ControllerExtensions
{
    public static Unit AddRefreshTokenToCookies(this ControllerBase controller, string refreshToken, DateTime dateTime)
    {
        controller.HttpContext.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = dateTime
        });
        return unit;
    }


    public static Unit RemoveRefreshTokenFromCookies(this ControllerBase controller)
    {
        controller.HttpContext.Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
        });
        return unit;
    }

    public static string? GetRefreshTokenFromCookies(this ControllerBase controller)
    {
        controller.HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken);
        return refreshToken;
    }
}
