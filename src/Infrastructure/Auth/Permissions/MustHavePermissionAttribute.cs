using Microsoft.AspNetCore.Authorization;
using Travaloud.Shared.Authorization;

namespace Travaloud.Infrastructure.Auth.Permissions;

public class MustHavePermissionAttribute : AuthorizeAttribute
{
    public MustHavePermissionAttribute(string action, string resource) =>
        Policy = TravaloudPermission.NameFor(action, resource);
}