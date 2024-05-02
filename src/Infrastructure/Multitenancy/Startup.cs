using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Travaloud.Application.Multitenancy;
using Travaloud.Infrastructure.Persistence;
using Travaloud.Shared.Authorization;
using Travaloud.Shared.Multitenancy;

namespace Travaloud.Infrastructure.Multitenancy;

internal static class Startup
{
    public static IServiceCollection AddMultitenancy(this IServiceCollection services, IConfiguration config)
    {
        // TODO: We should probably add specific dbprovider/connectionstring setting for the tenantDb with a fallback to the main databasesettings
        var databaseSettings = config.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
        var rootConnectionString = databaseSettings?.ConnectionString;
        if (string.IsNullOrEmpty(rootConnectionString))
            throw new InvalidOperationException("DB ConnectionString is not configured.");
        var dbProvider = databaseSettings?.DBProvider;
        if (string.IsNullOrEmpty(dbProvider)) throw new InvalidOperationException("DB Provider is not configured.");

        return services
            .AddDbContextFactory<TenantDbContext>(m => { m.UseDatabase(dbProvider, rootConnectionString); }
                , ServiceLifetime.Scoped)
            .AddMultiTenant<TravaloudTenantInfo>()
            .WithPerTenantOptions<IdentityOptions>((options, tenantInfo) => { options.User.RequireUniqueEmail = true; })
            .WithJsonStrategy(config)
            .WithClaimStrategy(TravaloudClaims.Tenant)
            .WithQueryStringStrategy(MultitenancyConstants.TenantIdName)
            .WithUrlStrategy()
            .WithEFCoreStore<TenantDbContext, TravaloudTenantInfo>()
            .Services
            .AddScoped<ITenantService, TenantService>();
    }

    internal static IApplicationBuilder UseMultiTenancy(this IApplicationBuilder app) =>
        app.UseMultiTenant();

    private static FinbuckleMultiTenantBuilder<TravaloudTenantInfo> WithQueryStringStrategy(
        this FinbuckleMultiTenantBuilder<TravaloudTenantInfo> builder, string queryStringKey) =>
        builder.WithDelegateStrategy(context =>
        {
            if (context is not HttpContext httpContext)
            {
                return Task.FromResult((string?) null);
            }

            httpContext.Request.Query.TryGetValue(queryStringKey, out var tenantIdParam);

            return Task.FromResult((string?) tenantIdParam.ToString());
        });

    private static FinbuckleMultiTenantBuilder<TravaloudTenantInfo> WithUrlStrategy(
        this FinbuckleMultiTenantBuilder<TravaloudTenantInfo> builder) =>
        builder.WithDelegateStrategy(context =>
        {
            if (context is not HttpContext httpContext)
            {
                return Task.FromResult((string?) null);
            }

            var fullUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}";

            // Use the full URL to determine the tenant
            var tenantIdentifier = GetTenantIdentifierFromUrl(fullUrl);

            return Task.FromResult(tenantIdentifier)!;
        });

    private static FinbuckleMultiTenantBuilder<TravaloudTenantInfo> WithJsonStrategy(
        this FinbuckleMultiTenantBuilder<TravaloudTenantInfo> builder, IConfiguration configuration) =>
        builder.WithDelegateStrategy(context =>
        {
            var tenantSettings = GetTenantSettings(configuration);

            return (tenantSettings != null ? Task.FromResult(tenantSettings.Identifier)! : Task.FromResult((string?) null)!)!;
        });


    private static string GetTenantIdentifierFromUrl(string url)
    {
        var host = new Uri(url).Host;

        var subdomains = host.Split('.');

        return subdomains.Length <= 1 ? "root" : subdomains[0];
    }

    private static TenantSettings? GetTenantSettings(IConfiguration config) =>
        config.GetSection(nameof(TenantSettings)).Get<TenantSettings>()!;
}