using Microsoft.AspNetCore.Authorization;
using Travaloud.Application.Common.Interfaces;
using Travaloud.Application.Identity.Users;
using Travaloud.Shared.Authorization;

namespace Travaloud.Infrastructure.Auth.Permissions;

internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IUserService _userService;
    private readonly ICurrentUser _currentUser;
    
    public PermissionAuthorizationHandler(IUserService userService, ICurrentUser currentUser) =>
        (_userService, _currentUser) = (userService, currentUser);

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var userId = context.User.Identity is {IsAuthenticated: false} ? _currentUser.GetUserId().ToString() : context.User?.GetUserId();
        
        if (!string.IsNullOrEmpty(userId) &&
            await _userService.HasPermissionAsync(userId, requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}