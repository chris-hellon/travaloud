using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Travaloud.Shared.Authorization;

namespace Travaloud.Infrastructure.Auth;

public static class AuthorizationServiceExtensions
{
    public static async Task<bool> HasPermissionAsync(this IAuthorizationService service, ClaimsPrincipal user, string action, string resource) =>
        (await service.AuthorizeAsync(user, null, TravaloudPermission.NameFor(action, resource))).Succeeded;
}