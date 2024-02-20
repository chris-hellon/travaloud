using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Travaloud.Application.Common.Interfaces;
using Travaloud.Infrastructure.Auth.Permissions;
using Travaloud.Infrastructure.Identity;

namespace Travaloud.Infrastructure.Auth;

internal static class Startup
{
    internal static IServiceCollection AddAuth(this IServiceCollection services, string cookieName)
    {
        services
            .AddCurrentUser()
            .AddPermissions()
            .AddIdentity(cookieName);
        
        return services;
    }

    internal static IApplicationBuilder UseCurrentUser(this IApplicationBuilder app) =>
        app.UseMiddleware<CurrentUserMiddleware>();

    private static IServiceCollection AddCurrentUser(this IServiceCollection services) =>
        services
            .AddScoped<CurrentUserMiddleware>()
            .AddScoped<ICurrentUser, CurrentUser>()
            .AddScoped(sp => (ICurrentUserInitializer)sp.GetRequiredService<ICurrentUser>());

    private static IServiceCollection AddPermissions(this IServiceCollection services)
    {
        // services.AddAuthorization(config =>
        // {
        //     foreach (var permission in TravaloudPermissions.All)
        //     {
        //         config.AddPolicy(permission.Name, policy =>
        //             policy.Requirements.Add(new PermissionRequirement(TravaloudPermission.NameFor(permission.Action, permission.Resource))));
        //     }
        // });
        //
        // services.AddAuthorizationBuilder()
        //     .AddPolicy("admin_greetings", policy =>
        //         policy
        //             .RequireRole("admin")
        //             .RequireClaim("scope", "greetings_api"));
        //
        //
        // foreach (var permission in TravaloudPermissions.All)
        // {
        //     services.AddAuthorizationBuilder()
        //         .add
        //         .AddPolicy("admin_greetings", policy =>
        //             policy
        //                 .RequireRole("admin")
        //                 .RequireClaim("scope", "greetings_api"));
        //
        // }

        services
            .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
            .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        return services;
    }
        
}