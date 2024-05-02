using Hangfire;
using Hangfire.SqlServer;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;

namespace Travaloud.Infrastructure.BackgroundJobs;

public static class Startup
{
    private static readonly ILogger _logger = Log.ForContext(typeof(Startup));

    internal static IServiceCollection AddBackgroundJobs(this IServiceCollection services, IConfiguration config)
    {
        services.AddHangfireServer(options => config.GetSection("HangfireSettings:Server").Bind(options));
        
        var storageSettings = config.GetSection("HangfireSettings:Storage").Get<HangfireStorageSettings>();
        if (storageSettings is null) throw new Exception("Hangfire Storage Provider is not configured.");
        if (string.IsNullOrEmpty(storageSettings.StorageProvider)) throw new Exception("Hangfire Storage Provider is not configured.");
        if (string.IsNullOrEmpty(storageSettings.ConnectionString)) throw new Exception("Hangfire Storage Provider ConnectionString is not configured.");
        _logger.Information("Hangfire: Current Storage Provider : {StorageSettingsStorageProvider}", storageSettings.StorageProvider);
        _logger.Information("For more Hangfire storage, visit https://www.hangfire.io/extensions.html");

        services.AddSingleton<JobActivator, TravaloudJobActivator>();

        GlobalConfiguration.Configuration.UseSerializerSettings(new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        });
        
        services.AddHangfire((provider, hangfireConfig) => hangfireConfig
            .UseDatabase(storageSettings.StorageProvider, storageSettings.ConnectionString, config)
            .UseFilter(new TravaloudJobFilter(provider))
            .UseFilter(new LogJobFilter()));

        return services;
    }

    private static IGlobalConfiguration UseDatabase(this IGlobalConfiguration hangfireConfig, string dbProvider, string connectionString, IConfiguration config) =>
        hangfireConfig.UseSqlServerStorage(connectionString, config.GetSection("HangfireSettings:Storage:Options").Get<SqlServerStorageOptions>());

    internal static IApplicationBuilder UseHangfireDashboard(this IApplicationBuilder app, IConfiguration config, IWebHostEnvironment env, bool isBlazor)
    {
        if (!env.IsDevelopment() || !isBlazor) return app;
        
        var dashboardOptions = config.GetSection("HangfireSettings:Dashboard").Get<DashboardOptions>();
        if (dashboardOptions is null) throw new Exception("Hangfire Dashboard is not configured.");
        dashboardOptions.Authorization = new[]
        {
            new HangfireCustomBasicAuthenticationFilter
            {
                User = config.GetSection("HangfireSettings:Credentials:User").Value,
                Pass = config.GetSection("HangfireSettings:Credentials:Password").Value
            }
        };

        return app.UseHangfireDashboard(config["HangfireSettings:Route"], dashboardOptions);

    }
}