namespace Travaloud.Admin.Configurations;

internal static class Startup
{
    internal static ConfigureHostBuilder AddConfigurations(this ConfigureHostBuilder host)
    {
#pragma warning disable ASP0013
        host.ConfigureAppConfiguration((context, config) =>
        {
            const string configurationsDirectory = "Configurations";
            var env = context.HostingEnvironment;
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/logger.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/logger.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/database.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/database.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/mail.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/mail.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/middleware.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/middleware.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/azurestorage.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/signalr.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/stripe.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
        });
#pragma warning restore ASP0013
        return host;
    }
}