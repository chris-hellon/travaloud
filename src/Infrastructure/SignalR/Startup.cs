using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Travaloud.Infrastructure.SignalR;

public static class Startup
{
    internal static ISignalRServerBuilder AddAzureSignalR(this IServiceCollection services, IConfiguration config)
    {
        return services.AddSignalR().AddAzureSignalR(options =>
        {
            options.ServerStickyMode = Microsoft.Azure.SignalR.ServerStickyMode.Required;
            options.ConnectionString = GetAzureSignalrSettings(config).ConnectionString;
            options.ClaimsProvider = context => context.User.Claims.Where(x => x.Type is ClaimTypes.NameIdentifier or ClaimTypes.Email);

        });
    }
    
    private static AzureSignalrSettings GetAzureSignalrSettings(IConfiguration config) =>
        config.GetSection(nameof(AzureSignalrSettings)).Get<AzureSignalrSettings>()!;
}