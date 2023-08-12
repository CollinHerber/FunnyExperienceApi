using Microsoft.AspNetCore.Http;
using NSec.Cryptography;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace FunnyExperience.Server.Lib.Extensions;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this HttpContext httpContext)
    {
        var userId = httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return !string.IsNullOrWhiteSpace(userId) ? Guid.Parse(userId) : Guid.Empty;
    }

    public static string GetUserIp(this HttpContext httpContext)
    {
        return httpContext?.Request?.Headers["X-Forwarded-For"].FirstOrDefault()?.Trim().Trim(',').Split(',').Select(s => s.Trim()).FirstOrDefault()
               ?? httpContext?.Connection?.RemoteIpAddress.ToString();
    }

    public static string GetUserAgent(this HttpContext httpContext)
    {
        return httpContext?.Request.Headers["User-Agent"];
    }
}