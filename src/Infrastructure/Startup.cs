using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Travaloud.Infrastructure.Auth;
using Travaloud.Infrastructure.Caching;
using Travaloud.Infrastructure.Common;
using Travaloud.Infrastructure.FileStorage;
using Travaloud.Infrastructure.Localization;
using Travaloud.Infrastructure.Mailing;
using Travaloud.Infrastructure.Mapping;
using Travaloud.Infrastructure.Middleware;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Infrastructure.Persistence;
using Travaloud.Infrastructure.Persistence.Initialization;

namespace Travaloud.Infrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        MapsterSettings.Configure();
        return services
            .AddAuth(config)
            .AddCaching(config)
            .AddExceptionMiddleware()
            .AddLocalization(config)
            .AddMailing(config)
            .AddAzureStorage(config)
            .AddMediatR(cfg=>cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()))
            .AddMultitenancy(config)
            .AddPersistence(config)
            .AddRequestLogging(config)
            .AddHttpContextAccessor()
            .AddHttpClient()
            .AddRouting(options => options.LowercaseUrls = true)
            .AddServices();
    }
    
    public static async Task InitializeDatabasesAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        // Create a new scope to retrieve scoped services
        using var scope = services.CreateScope();

        await scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>()
            .InitializeDatabasesAsync(cancellationToken);
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder, IConfiguration config, IWebHostEnvironment env) =>
        builder
            .UseHttpsRedirection()
            .UseLocalization(config)
            .UseStaticFiles()
            .UseFileStorage(env)
            .UseExceptionMiddleware()
            .UseRouting()
            .UseAntiforgery()
            .UseAuthentication()
            .UseCurrentUser()
            .UseMultiTenancy()
            .UseAuthorization()
            .UseRequestLogging(config);
}