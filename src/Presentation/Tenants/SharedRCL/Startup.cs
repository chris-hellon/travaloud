using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Travaloud.Infrastructure;

namespace Travaloud.Tenants.SharedRCL;

public static class Startup
{
    public static void AddTenantWebsiteInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env, string cookieName)
    {
        services.AddInfrastructure(configuration, cookieName, false);
        Infrastructure.Multitenancy.TenantWebsite.Startup.AddTenantWebsite(services, configuration, env);
    }

    public static void UseTenantWebsiteInfrastructure(this IApplicationBuilder app, IConfiguration configuration, IWebHostEnvironment env)
    {
        app.UseInfrastructure(configuration, env);
        Infrastructure.Multitenancy.TenantWebsite.Startup.UseTenantWebsite(app, env);
    }
}