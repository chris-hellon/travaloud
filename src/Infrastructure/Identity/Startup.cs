using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Travaloud.Infrastructure.Persistence.Context;

namespace Travaloud.Infrastructure.Identity;

public static class Startup
{
    internal static IServiceCollection AddIdentity(this IServiceCollection services, string cookieName, bool isBlazor)
    {
        if (isBlazor)
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
                        applcationCookieOptions.Cookie.Name =$"{cookieName}-auth";
                    });
                });
        }
        else
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.User.RequireUniqueEmail = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 4;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                }).AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager<SignInManager<ApplicationUser>>()
                .AddUserManager<UserManager<ApplicationUser>>().AddRoles<ApplicationRole>().AddDefaultUI().AddDefaultTokenProviders();
            
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.MaxAge = TimeSpan.FromDays(365);
                options.Cookie.HttpOnly = false;
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                options.SlidingExpiration = true;
                options.Cookie.Name = $"{cookieName}-auth";
            });
        }

        return services;
    }
}