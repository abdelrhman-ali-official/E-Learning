using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace Start.Middlewares;

/// <summary>
/// Rate limiting middleware for video access endpoints
/// Limits to 30 video requests per minute per user
/// </summary>
public class VideoRateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private const int MaxRequestsPerMinute = 30;
    private static readonly TimeSpan TimeWindow = TimeSpan.FromMinutes(1);

    public VideoRateLimitingMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only apply rate limiting to video stream endpoints
        var path = context.Request.Path.Value?.ToLower() ?? string.Empty;
        var isVideoEndpoint = path.Contains("/videos/") && !path.Contains("/progress") ||
                              path.Contains("/live") && context.Request.Method == "GET";

        if (!isVideoEndpoint)
        {
            await _next(context);
            return;
        }

        // Get user ID from claims
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            await _next(context);
            return;
        }

        var cacheKey = $"VideoRateLimit_{userId}";
        
        if (!_cache.TryGetValue(cacheKey, out int requestCount))
        {
            requestCount = 0;
        }

        if (requestCount >= MaxRequestsPerMinute)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Rate limit exceeded",
                message = $"Maximum {MaxRequestsPerMinute} video requests per minute allowed"
            });
            return;
        }

        // Increment request count
        requestCount++;
        _cache.Set(cacheKey, requestCount, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeWindow
        });

        await _next(context);
    }
}
