using System.Globalization;
using System.IO.Compression;
using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rollbar;
using Rollbar.NetCore.AspNet;
using Travaloud.Application.Common.Interfaces;
using Travaloud.Infrastructure.Common.Services;

namespace Travaloud.Infrastructure.Multitenancy.TenantWebsite;

public static class Startup
{
    public static IServiceCollection AddTenantWebsite(this IServiceCollection services, IConfiguration config)
    {
        var travaloudSettings = services.GetTravaloudSettings(config);

        var propertyBookingUrl = "property-booking";
        var tourBookingUrl = "tour-booking";
        var tourUrl = "tour";
        var tourCategoryUrl = string.Empty;

        var urlSection = travaloudSettings?.UrlConfiguration;

        if (urlSection != null)
        {
            propertyBookingUrl = urlSection.PropertyBookingUrl;
            tourBookingUrl = urlSection.TourBookingUrl;
            
            if (!string.IsNullOrEmpty(urlSection.TourUrl))
                tourUrl = $"{urlSection.TourUrl}";
            
            if (!string.IsNullOrEmpty(urlSection.TourCategoryUrl))
                tourCategoryUrl = $"{urlSection.TourCategoryUrl}";   
        }

        services.AddRazorPages().AddRazorRuntimeCompilation().AddRazorPagesOptions(options =>
        {
            options.Conventions.AddPageRoute("/PropertyBooking/Index", propertyBookingUrl + "/{propertyName}/{checkInDate?}/{checkOutDate?}/{userId?}");
            options.Conventions.AddPageRoute("/TourBooking/Index", tourBookingUrl);
            options.Conventions.AddPageRoute("/Tour/Index", tourUrl + "/{tourName}");
            options.Conventions.AddPageRoute("/JoinOurCrew/Index", "join-our-crew/{tourName?}");
            options.Conventions.AddPageRoute("/TravelGuides/Index", "travel-guides/{pageNumber:int?}");
            options.Conventions.AddPageRoute("/TravelGuide/Index", "travel-guides/{title}");

            if (!string.IsNullOrEmpty(tourCategoryUrl))
            {
                options.Conventions.AddPageRoute("/TourCategory/Index", tourCategoryUrl + "/{tourCategoryName?}");
            }
        });
        
        services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            }).AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = Enumerable.Empty<string>();
                options.MimeTypes.Append("image/png");
                options.MimeTypes.Append("image/webp");
                options.MimeTypes.Append("image/jpg");
                options.MimeTypes.Append("image/jpeg");
            }).Configure<BrotliCompressionProviderOptions>(options => { options.Level = CompressionLevel.Fastest; })
            .Configure<GzipCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; })
            .AddSession()
            .AddResponseCaching()
            .AddDetection();
        
        services.AddScoped<IRazorPartialToStringRenderer, RazorPartialToStringRenderer>();

        var rollbarSettings = services.GetRollbarSettings(config);
        
        if (rollbarSettings != null)
            services.AddRolbarConfiguration(rollbarSettings);
        
        var reCaptchaSettings = services.GetReCaptchaSettings(config);
        
        if (reCaptchaSettings != null)
        {
            services.AddReCaptcha(options =>
            {
                options.SiteKey = reCaptchaSettings.SiteKey;
                options.SecretKey = reCaptchaSettings.SecretKey;
                options.Version = ReCaptchaVersion.V2;
            });
        }

        return services;
    }

    public static IApplicationBuilder UseTenantWebsite(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/error");
            app.UseHsts();
            app.UseRollbarMiddleware();
        }

        //app.UseStatusCodePagesWithRedirects("/error/{0}");

        app.SetAppCulture();
        app.UseSession();
        
        return app;
    }
    
    private static void SetAppCulture(this IApplicationBuilder app)
    {
        var culture = CultureInfo.CreateSpecificCulture("en-GB");
        var dateformat = new DateTimeFormatInfo
        {
            ShortDatePattern = "dd/MM/yyyy",
            LongDatePattern = "dd/MM/yyyy hh:mm:ss tt"
        };
        culture.DateTimeFormat = dateformat;

        var supportedCultures = new[] {
            culture
        };

        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(culture),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        });
    }
    
    private static void AddRolbarConfiguration(this IServiceCollection services, RollbarSettings rollbarSettings)
    {
        var config = new RollbarInfrastructureConfig(rollbarSettings.AccessToken, rollbarSettings.Environment)
            {
                RollbarInfrastructureOptions =
                {
                    CaptureUncaughtExceptions = true
                }
            };
        var dataSecurityOptions = new RollbarDataSecurityOptions()
        {
            ScrubFields = new string[] { "url", "method", }
        };
        config.RollbarLoggerConfig.RollbarDataSecurityOptions.Reconfigure(dataSecurityOptions);

        RollbarInfrastructure.Instance.Init(config);

        services.AddRollbarLogger(loggerOptions =>
        {
            loggerOptions.Filter = (_, loglevel) => loglevel >= LogLevel.Error;
        });
    }

    private static ReCaptchaSettings? GetReCaptchaSettings(this IServiceCollection services, IConfiguration config)
    {
        var section = config.GetSection(nameof(ReCaptchaSettings));
        
        if (section == null)
            throw new Exception("No ReCaptchaSettings provided in appsettings.json.");

        services.Configure<ReCaptchaSettings>(section);

        return section.Get<ReCaptchaSettings>();
    }

    private static RollbarSettings? GetRollbarSettings(this IServiceCollection services, IConfiguration config)
    {
        var section = config.GetSection(nameof(RollbarSettings));
        
        if (section == null)
            throw new Exception("No RollbarSettings provided in appsettings.json.");

        services.Configure<RollbarSettings>(section);

        return section.Get<RollbarSettings>();
    }
    
    private static TravaloudSettings? GetTravaloudSettings(this IServiceCollection services, IConfiguration config)
    {
        var section = config.GetSection(nameof(TravaloudSettings));
        
        if (section == null)
            throw new Exception("No TravaloudSettings provided in appsettings.json.");

        services.Configure<TravaloudSettings>(section);

        return section.Get<TravaloudSettings>();
    }
}