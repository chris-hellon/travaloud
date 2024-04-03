using System.Globalization;
using System.IO.Compression;
using AspNetCore.ReCaptcha;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using JavaScriptEngineSwitcher.V8;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NUglify.Helpers;
using Rollbar;
using Rollbar.NetCore.AspNet;
using Travaloud.Application.Common.Interfaces;
using Travaloud.Infrastructure.Common.Services;

namespace Travaloud.Infrastructure.Multitenancy.TenantWebsite;

public static class Startup
{
    public static IServiceCollection AddTenantWebsite(this IServiceCollection services, IConfiguration config,
        IHostEnvironment hostEnvironment)
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

        services.AddRazorPages().AddRazorRuntimeCompilation(options =>
        {
            var libraryPath = Path.GetFullPath(Path.Combine(hostEnvironment.ContentRootPath, "..", "SharedRCL"));
            options.FileProviders.Add(new PhysicalFileProvider(libraryPath));
        }).AddRazorPagesOptions(options =>
        {
            options.Conventions.AddPageRoute("/PropertyBooking/Index",
                propertyBookingUrl + "/{propertyName}/{checkInDate?}/{checkOutDate?}/{userId?}");
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

        services.AddJsEngineSwitcher(options => options.DefaultEngineName = V8JsEngine.EngineName)
            .AddV8();


        var libraryPath = Path.GetFullPath(Path.Combine(hostEnvironment.ContentRootPath, "..", "SharedRCL", "wwwroot"));

        services.AddWebOptimizer(pipeline =>
        {
            var provider = new PhysicalFileProvider(libraryPath);

            pipeline.AddScssBundle("/css/theme.min.css",
                    "/mdb/src/scss/mdb.pro.scss")
                .MinifyCss();

            pipeline.AddScssBundle("/shared/css/theme.min.css",
                    "/css/_additional.scss",
                    "/css/_daterangepicker.scss",
                    "/lib/owlcarousel/owlcarousel.min.css",
                    "/lib/owlcarousel/owlcarousel.theme.min.css")
                .MinifyCss()
                .UseFileProvider(provider);

            pipeline.AddJavaScriptBundle("/js/theme.min.js",
                    "/mdb/js/mdb.umd.min.js",
                    "/js/site.min.js"
                )
                .Concatenate()
                .MinifyJavaScript();
            
            pipeline.AddJavaScriptBundle("/shared/js/theme.min.js",
                    "/lib/jquery/dist/jquery.min.js",
                    "/lib/cookie.min.js",
                    "/lib/owlcarousel/owl.carousel.min.js",
                    "/lib/moment.min.js",
                    "/lib/daterangepicker.js",
                    "/js/daterangepickeraddition.min.js",
                    "/js/travaloud.min.js",
                    "/js/videoplayer.min.js",
                    "/js/classes.min.js"
                )
                .Concatenate()
                .MinifyJavaScript()
                .UseFileProvider(provider);
            
            pipeline.AddJavaScriptBundle("/shared/js/propertybooking.min.js",
                    "/js/propertybooking.js"
                )
                .MinifyJavaScript()
                .UseFileProvider(provider);
            
            pipeline.AddJavaScriptBundle("/shared/js/property.min.js",
                    "/js/propertybooking.js"
                )
                .MinifyJavaScript()
                .UseFileProvider(provider);
            
            pipeline.AddJavaScriptBundle("/shared/js/basket.min.js",
                    "/js/basket.js"
                )
                .MinifyJavaScript()
                .UseFileProvider(provider);

            pipeline.AddJavaScriptBundle("/shared/js/checkout.min.js",
                    "/js/checkout.js"
                )
                .MinifyJavaScript()
                .UseFileProvider(provider);
            
            pipeline.AddJavaScriptBundle("/shared/js/tour.min.js",
                    "/js/tour.js"
                )
                .MinifyJavaScript()
                .UseFileProvider(provider);
            
            pipeline.CompileScssFiles().MinifyCss();
        });


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

        app.UseWebOptimizer();

        //var libraryPath = Path.GetFullPath(Path.Combine(env.ContentRootPath, "..", "SharedRCL", "wwwroot"));
        //app.UseStaticFiles();
        // app.UseStaticFiles(new StaticFileOptions
        // {
        //     FileProvider = new PhysicalFileProvider(libraryPath),
        //     RequestPath = new PathString("/shared")
        // });
        //
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

        var supportedCultures = new[]
        {
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
            ScrubFields = new string[] {"url", "method",}
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