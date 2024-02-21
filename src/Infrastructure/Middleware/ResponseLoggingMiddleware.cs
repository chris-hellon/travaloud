using Finbuckle.MultiTenant;
using Serilog;
using Serilog.Context;
using Travaloud.Application.Common.Interfaces;
using Travaloud.Infrastructure.Multitenancy;

namespace Travaloud.Infrastructure.Middleware;

public class ResponseLoggingMiddleware : IMiddleware
{
    private readonly ICurrentUser _currentUser;
    private readonly IMultiTenantContextAccessor<TravaloudTenantInfo> _multiTenantContextAccessor;
    
    public ResponseLoggingMiddleware(ICurrentUser currentUser, IMultiTenantContextAccessor<TravaloudTenantInfo> multiTenantContextAccessor)
    {
        _currentUser = currentUser;
        _multiTenantContextAccessor = multiTenantContextAccessor;
    }

    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        await next(httpContext);
        var originalBody = httpContext.Response.Body;
        using var newBody = new MemoryStream();
        httpContext.Response.Body = newBody;
        string responseBody;
        if (httpContext.Request.Path.ToString().Contains("tokens"))
        {
            responseBody = "[Redacted] Contains Sensitive Information.";
        }
        else
        {
            newBody.Seek(0, SeekOrigin.Begin);
            responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        }

        var email = _currentUser.GetUserEmail() is string userEmail ? userEmail : "Anonymous";
        var userId = _currentUser.GetUserId();
        var tenant = _multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Name ?? string.Empty;
        if (userId != DefaultIdType.Empty) LogContext.PushProperty("UserId", userId);
        LogContext.PushProperty("UserEmail", email);
        if (!string.IsNullOrEmpty(tenant)) LogContext.PushProperty("Tenant", tenant);
        LogContext.PushProperty("StatusCode", httpContext.Response.StatusCode);
        LogContext.PushProperty("ResponseTimeUTC", DateTime.UtcNow);
        Log.ForContext("ResponseHeaders", httpContext.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), destructureObjects: true)
       .ForContext("ResponseBody", responseBody)
       .Information("HTTP {RequestMethod} Request to {RequestPath} by {RequesterEmail} has Status Code {StatusCode}.", httpContext.Request.Method, httpContext.Request.Path, string.IsNullOrEmpty(email) ? "Anonymous" : email, httpContext.Response.StatusCode);
        newBody.Seek(0, SeekOrigin.Begin);
        await newBody.CopyToAsync(originalBody);
    }
}