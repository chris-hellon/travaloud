using FuseHostelsAndTravel.Configurations;
using Microsoft.AspNetCore.Rewrite;
using Serilog;
using Travaloud.Application;
using Travaloud.Infrastructure;
using Travaloud.Infrastructure.Common;
using Travaloud.Tenants.SharedRCL;

StaticLogger.EnsureInitialized();
Log.Information("Server Booting Up...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.AddConfigurations();
    builder.Host.UseSerilog((_, config) =>
    {
        config.WriteTo.Console()
            .ReadFrom.Configuration(builder.Configuration);
    });
    
    builder.Services.AddTenantWebsiteInfrastructure(builder.Configuration, builder.Environment, "FuseHostelsAndTravel");
    builder.Services.AddApplication();
    
    var app = builder.Build();

    await app.Services.InitializeDatabasesAsync();

    app.UseTenantWebsiteInfrastructure(builder.Configuration, builder.Environment);
    app.MapRazorPages();

    var rewriteOptions = new RewriteOptions().AddRedirectToNonWwwPermanent();
    app.UseRewriter(rewriteOptions);
    
    Log.Information("Application running successfully");

    app.Use(async (context, next) =>
    {
        var tenantWebsiteService = context.RequestServices.GetRequiredService<ITenantWebsiteService>();
        var redirects = await tenantWebsiteService.GetSeoRedirects(new CancellationToken());
        
        var requestUrl = context.Request.Path.Value;
        foreach (var redirect in from redirect in redirects
                 let normalizedRedirectUrl = redirect.Url.TrimStart('/')
                 let normalizedRequestUrl = requestUrl.TrimStart('/')
                 where string.Equals(normalizedRequestUrl,
                     normalizedRedirectUrl,
                     StringComparison.OrdinalIgnoreCase)
                 select redirect)
        {
            context.Response.Redirect(redirect.RedirectUrl, true);
            return;
        }

        await next();
    });
    app.Run();
}
catch (Exception ex) when (!ex.GetType().Name.Equals("StopTheHostException", StringComparison.Ordinal))
{
    StaticLogger.EnsureInitialized();
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    StaticLogger.EnsureInitialized();
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}