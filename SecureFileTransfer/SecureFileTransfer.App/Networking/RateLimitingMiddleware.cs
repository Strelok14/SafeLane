using Microsoft.AspNetCore.Http;
using SecureFileTransfer.App.Infrastructure;
using SecureFileTransfer.Core.Protection;

namespace SecureFileTransfer.App.Networking;

public sealed class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;

    public RateLimitingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IRateLimiter limiter, AppSettings settings)
    {
        if (context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            if (!limiter.IsAllowed(ip, settings.MaxRequestsPerSecond))
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Rate limit exceeded");
                return;
            }
        }

        await _next(context);
    }
}
