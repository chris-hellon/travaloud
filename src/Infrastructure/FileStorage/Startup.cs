using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Travaloud.Infrastructure.FileStorage;

internal static class Startup
{
    internal static IApplicationBuilder UseFileStorage(this IApplicationBuilder app, IWebHostEnvironment env) =>
        app.UseStaticFiles(new StaticFileOptions()
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(env.WebRootPath, "Files")),
            RequestPath = new PathString("/Files")
        });

    internal static IServiceCollection AddAzureStorage(this IServiceCollection services, IConfiguration config) =>
       services.Configure<AzureStorageSettings>(config.GetSection(nameof(AzureStorageSettings)));
}