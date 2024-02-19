using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Travaloud.Application.Common.Caching;

namespace Travaloud.Infrastructure.Caching;

internal static class Startup
{
    internal static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration config)
    {
        var settings = config.GetSection(nameof(CacheSettings)).Get<CacheSettings>();

        if (settings is {UseDistributedCache: true})
        {
            services.AddDistributedMemoryCache();
            services.AddTransient<ICacheService, DistributedCacheService>();
        }
        else
        {
            services.AddMemoryCache();
            services.AddTransient<ICacheService, LocalCacheService>();
        }

        return services;
    }
}