using System.Globalization;
using System.Reflection;
using AspNetCore.SEOHelper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Travaloud.Infrastructure.Auth;
using Travaloud.Infrastructure.BackgroundJobs;
using Travaloud.Infrastructure.Caching;
using Travaloud.Infrastructure.Cloudbeds;
using Travaloud.Infrastructure.Common;
using Travaloud.Infrastructure.FileStorage;
using Travaloud.Infrastructure.Identity;
using Travaloud.Infrastructure.Localization;
using Travaloud.Infrastructure.Mailing;
using Travaloud.Infrastructure.Mapping;
using Travaloud.Infrastructure.Middleware;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Infrastructure.Persistence;
using Travaloud.Infrastructure.Persistence.Context;
using Travaloud.Infrastructure.Persistence.Initialization;
using Travaloud.Infrastructure.PaymentProcessing;

namespace Travaloud.Infrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config,
        string cookieName, bool isBlazor)
    {
        MapsterSettings.Configure();
        services
            .AddAuth(cookieName, isBlazor)
            
            .AddCaching(config)
            .AddExceptionMiddleware()
            .AddLocalization(config)
            .AddMailing(config)
            .AddAzureStorage(config)
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()))
            .AddMultitenancy(config)
            .AddPersistence(config)
            .AddRequestLogging(config)
            .AddHttpContextAccessor()
            .AddStripe(config)
            .AddMultiTenantStripe(config)
            .AddRouting(options => options.LowercaseUrls = true)
            .AddCloudbedsHttpClient(config);

        services.AddBackgroundJobs(config);
        
        if (isBlazor)
        {
            services.AddScoped<UserManager<ApplicationUser>, UserManager<ApplicationUser>>();
            
            services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<ApplicationRole>()
                .AddSignInManager()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();
        }
        
        services.AddServices();

        return services;
    }

    public static async Task InitializeDatabasesAsync(this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        // Create a new scope to retrieve scoped services
        using var scope = services.CreateScope();

        await scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>()
            .InitializeDatabasesAsync(cancellationToken);
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder, IConfiguration config,
        IWebHostEnvironment env, bool isBlazor) =>
        builder
            .UseHttpsRedirection()
            .UseLocalization(config)
            .UseStaticFiles()
            .UseExceptionMiddleware()
            .UseRouting()
            .UseAntiforgery()
            .UseMultiTenancy()
            .UseAuthentication()
            .UseCurrentUser()
            .UseAuthorization()
            .UseRequestLogging(config)
            .UseHangfireDashboard(config, env, isBlazor)
            .SetAppCulture();
    
    private static IApplicationBuilder SetAppCulture(this IApplicationBuilder app)
    {
        var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Add("X-Time-Zone", vietnamTimeZone.Id); // Optionally, set the time zone header
            context.Response.Headers.Add("X-Time-Zone-Offset", vietnamTimeZone.BaseUtcOffset.ToString()); // Optionally, set the time zone offset header
            await next();
        });

        var culture = new CultureInfo("en-US")
        {
            DateTimeFormat =
            {
                ShortDatePattern = "dd/MM/yyyy", // Set short date pattern
                LongDatePattern = "dd/MM/yyyy hh:mm:ss tt" // Set long date pattern
            }
        }; // Use "en-US" for English

        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(culture), // Default culture is set
            SupportedCultures = new[] { culture }, // Supported cultures are set
            SupportedUICultures = new[] { culture } // Supported UI cultures are set
        });

        return app;
    }
}