using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Travaloud.Infrastructure.Identity;

public static class Startup
{
    internal static IServiceCollection AddIdentity(this IServiceCollection services, string cookieName)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies(options =>
            {
                options.ApplicationCookie?.Configure(applcationCookieOptions =>
                {
                    applcationCookieOptions.LoginPath = "/account/login";
                    applcationCookieOptions.AccessDeniedPath = "/access-denied";
                    applcationCookieOptions.ExpireTimeSpan = TimeSpan.FromHours(12);
                    applcationCookieOptions.SlidingExpiration = true;
                    applcationCookieOptions.Cookie.Name = cookieName;
                });
            });
        
        return services;
    }
}