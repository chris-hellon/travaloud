using Travaloud.Application.Catalog.Interfaces;

namespace Travaloud.Infrastructure.Middleware;

public class DynamicRedirectMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITenantWebsiteService _tenantWebsiteService;

    public DynamicRedirectMiddleware(RequestDelegate next, ITenantWebsiteService tenantWebsiteService)
    {
        _next = next;
        _tenantWebsiteService = tenantWebsiteService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Retrieve the current request URL
        var requestUrl = context.Request.Path.Value;

        // Check for non-www to www redirect
        if (requestUrl.StartsWith("/www.", StringComparison.OrdinalIgnoreCase))
        {
            var nonWwwUrl = requestUrl.Replace("/www.", "/", StringComparison.OrdinalIgnoreCase);
            context.Response.Redirect(nonWwwUrl, true);
            return;
        }

        // Fetch SEO redirects
        var redirects = await _tenantWebsiteService.GetSeoRedirects(new CancellationToken());

        // Check for SEO redirects
        foreach (var redirect in redirects)
        {
            if (string.Equals(requestUrl, redirect.Url, StringComparison.OrdinalIgnoreCase))
            {
                context.Response.Redirect(redirect.RedirectUrl, true);
                return;
            }
        }

        // Continue to the next middleware if no redirect occurred
        await _next(context);
    }
}