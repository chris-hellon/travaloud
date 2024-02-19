using Microsoft.Extensions.DependencyInjection;

namespace Travaloud.Infrastructure.Identity;

public static class Startup
{
    internal static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        // services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
        //     .AddRoles<ApplicationRole>()
        //     .AddSignInManager()
        //     .AddDefaultTokenProviders()
        //     .AddEntityFrameworkStores<ApplicationDbContext>();

        return services;
    }
}