using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Travaloud.Application.Identity.Users;
using Travaloud.Domain.Identity;
using Travaloud.Shared.Authorization;
using Travaloud.Shared.Multitenancy;

namespace Travaloud.Infrastructure.Identity;

internal partial class UserService
{
    public async Task<List<UserRoleDto>> GetRolesAsync(string userId)
    {
        var userRoles = new List<UserRoleDto>();

        await using var dbContext = CreateDbContext();
        
        var user = await GetUserById(dbContext, userId);
        var roles = await dbContext.Roles.AsNoTracking().ToListAsync();
        foreach (var role in roles)
        {
            userRoles.Add(new UserRoleDto
            {
                RoleId = role.Id,
                RoleName = role.Name,
                Description = role.Description,
                Enabled = await IsUserInRole(dbContext, user, role.Name!)
            });
        }

        return userRoles;
    }

    public async Task<bool> UserIsInRole(string userId, string roleName)
    {
        await using var dbContext = CreateDbContext();

        var role = dbContext.Roles.FirstOrDefault(x => x.Name == roleName);

        if (role == null) return false;

        return dbContext.UserRoles.FirstOrDefault(x => x.UserId == userId && x.RoleId == role.Id) != null;
    }

    public async Task<string> AssignRolesAsync(string userId, UserRolesRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        await using var dbContext = CreateDbContext();
        var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);

        _ = user ?? throw new NotFoundException(_localizer["User Not Found."]);

        // Check if the user is an admin for which the admin role is getting disabled
        if (await IsUserInRole(dbContext, user, TravaloudRoles.Admin)
            && request.UserRoles.Any(a => a is {Enabled: false, RoleName: TravaloudRoles.Admin}))
        {
            // Get count of users in Admin Role
            var adminCount = (await GetUsersInRole(dbContext, TravaloudRoles.Admin)).Count;

            // Check if user is not Root Tenant Admin
            // Edge Case : there are chances for other tenants to have users with the same email as that of Root Tenant Admin. Probably can add a check while User Registration
            if (user.Email == MultitenancyConstants.Root.EmailAddress)
            {
                if (_currentTenant.Id == MultitenancyConstants.Root.Id)
                {
                    throw new ConflictException(_localizer["Cannot Remove Admin Role From Root Tenant Admin."]);
                }
            }
            else if (adminCount <= 2)
            {
                throw new ConflictException(_localizer["Tenant should have at least 2 Admins."]);
            }
        }

        foreach (var userRole in request.UserRoles)
        {
            // Check if Role Exists
            if (await dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.Name == userRole.RoleName) is null) continue;
            
            if (userRole.Enabled)
            {
                if (userRole.RoleName != null && await IsUserInRole(dbContext, user, userRole.RoleName)) continue;
                
                var role = await dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.Name == userRole.RoleName);
                await dbContext.UserRoles.AddAsync(new IdentityUserRole<string>()
                {
                    UserId = user.Id,
                    RoleId = role!.Id
                });
            }
            else
            {
                var dbUserRole = await dbContext.UserRoles.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == user.Id && x.RoleId == userRole.RoleId);

                if (dbUserRole != null) dbContext.UserRoles.Remove(dbUserRole);
            }
        }
        
        await dbContext.SaveChangesAsync();

        await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id, true));

        return _localizer["User Roles Updated Successfully."];
    }
}