using FuseHostelsAndTravel.Configurations;
using Serilog;
using Travaloud.Application;
using Travaloud.Infrastructure;
using Travaloud.Infrastructure.Common;
using Travaloud.Tenants.SharedRCL;

StaticLogger.EnsureInitialized();
Log.Information("Server Booting Up...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.AddConfigurations();
    builder.Host.UseSerilog((_, config) =>
    {
        config.WriteTo.Console()
            .ReadFrom.Configuration(builder.Configuration);
    });
    
    builder.Services.AddTenantWebsiteInfrastructure(builder.Configuration, "FuseHostelsAndTravel");
    builder.Services.AddApplication();
    
    var app = builder.Build();

    await app.Services.InitializeDatabasesAsync();

    app.UseTenantWebsiteInfrastructure(builder.Configuration, builder.Environment);
    app.MapRazorPages();
    
    Log.Information("Application running successfully");

    app.Run();
}
catch (Exception ex) when (!ex.GetType().Name.Equals("StopTheHostException", StringComparison.Ordinal))
{
    StaticLogger.EnsureInitialized();
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    StaticLogger.EnsureInitialized();
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}