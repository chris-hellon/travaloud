using Microsoft.EntityFrameworkCore;
using Travaloud.Application.Common.Caching;
using Travaloud.Shared.Authorization;

namespace Travaloud.Infrastructure.Identity;

internal partial class UserService
{
    public async Task<List<string>> GetPermissionsAsync(string userId, CancellationToken cancellationToken)
    {
        await using var dbContext = CreateDbContext();
        var user = await GetUserById(dbContext, userId);

        _ = user ?? throw new NotFoundException(_localizer["User Not Found."]);

        var userRoles = await GetUserRoles(dbContext, userId);
        var userRolesNames = userRoles.Select(x => x.RoleId).ToList();
        var permissions = new List<string>();
        var roles = await dbContext.Roles.AsNoTracking().Where(x => userRolesNames.Contains(x.Id)).ToListAsync(cancellationToken: cancellationToken);
        
        foreach (var role in roles)
        {
            permissions.AddRange((await dbContext.RoleClaims
                .Where(rc => rc.RoleId == role.Id && rc.ClaimType == TravaloudClaims.Permission)
                .Select(rc => rc.ClaimValue)
                .ToListAsync(cancellationToken))!);
        }

        return permissions.Distinct().ToList();
    }

    public async Task<bool> HasPermissionAsync(string userId, string permission, CancellationToken cancellationToken)
    {
        var permissions = await _cache.GetOrSetAsync(
            _cacheKeys.GetCacheKey(TravaloudClaims.Permission, userId),
            () => GetPermissionsAsync(userId, cancellationToken),
            cancellationToken: cancellationToken);

        return permissions?.Contains(permission) ?? false;
    }

    public Task InvalidatePermissionCacheAsync(string userId, CancellationToken cancellationToken) =>
        _cache.RemoveAsync(_cacheKeys.GetCacheKey(TravaloudClaims.Permission, userId), cancellationToken);
}