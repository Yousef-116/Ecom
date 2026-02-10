using System;
using System.Diagnostics.Metrics;
using System.Net;
using Ecom.API.Helper;
using Microsoft.Extensions.Caching.Memory;

namespace Ecom.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment environment;
    private readonly IMemoryCache memoryCache;
    private readonly TimeSpan rateLimitWindow = TimeSpan.FromSeconds(30);
    public ExceptionMiddleware(RequestDelegate next, IHostEnvironment environment, IMemoryCache memoryCache)
    {

        _next = next;
        this.environment = environment;
        this.memoryCache = memoryCache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            ApplySecurity(context);
            if (IsRequestAllowed(context) == false)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(
                    new ApiExceptions((int)HttpStatusCode.TooManyRequests,
                     "Too Many Request . pleas try again later !!!!!"));

                return; // 🚨 VERY IMPORTANT
            }
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            context.Response.ContentType = "application/json";

            var response = environment.IsDevelopment() ?
            new ApiExceptions((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace)
            : new ApiExceptions((int)HttpStatusCode.InternalServerError, ex.Message);

            await context.Response.WriteAsJsonAsync(response);

        }
    }

    private bool IsRequestAllowed(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress.ToString();
        var cashKey = $"Rate:{ip}";
        var dateNow = DateTime.Now;

        var (timesTamp, count) = memoryCache.GetOrCreate(cashKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = rateLimitWindow;
            return (timesTamp: dateNow, count: 0);
        });

        if (dateNow - timesTamp < rateLimitWindow)
        {
            if (count >= 8)
            {
                return false;
            }
            memoryCache.Set(cashKey, (timesTamp, count += 1), rateLimitWindow);

        }
        else
        {
            memoryCache.Set(cashKey, (timesTamp, count), rateLimitWindow);
        }
        return true;
    }

    private void ApplySecurity(HttpContext content)
    {
        content.Response.Headers["X-Content-Type-Options"] = "nosniff";
        content.Response.Headers["X-XSS-Protection"] = "1;mode=block";
        content.Response.Headers["X-Frame-Options"] = "DENY";
        content.Response.Headers["Content-Security-Policy"] =
            "default-src 'self'; img-src 'self' data:; script-src 'self'";
        content.Response.Headers["Referrer-Policy"] = "no-referrer";
        content.Response.Headers["Strict-Transport-Security"] =
            "max-age=31536000; includeSubDomains";
    }
}
