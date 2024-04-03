using System.Security.Claims;
using Finbuckle.MultiTenant;
using Travaloud.Application.Common.Interfaces;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Shared.Authorization;

namespace Travaloud.Infrastructure.Auth;

public class CurrentUser : ICurrentUser, ICurrentUserInitializer
{
    private readonly IMultiTenantContextAccessor<TravaloudTenantInfo>? _multiTenantContextAccessor;
    private TravaloudTenantInfo? _currentTenant;

    public CurrentUser(IMultiTenantContextAccessor<TravaloudTenantInfo>? multiTenantContextAccessor)
    {
        _multiTenantContextAccessor = multiTenantContextAccessor;
    }
    
    public TravaloudTenantInfo? CurrentTenant
    {
        get
        {
            if (_currentTenant != null)
                return _currentTenant;

            _currentTenant = _multiTenantContextAccessor?.MultiTenantContext?.TenantInfo;
            return _currentTenant;
        }
    }
    
    private ClaimsPrincipal? _user;

    public string? Name => _user?.Identity?.Name;

    private DefaultIdType _userId = DefaultIdType.Empty;
    
    public DefaultIdType GetUserId() =>
        IsAuthenticated()
            ? DefaultIdType.Parse(_user?.GetUserId() ?? DefaultIdType.Empty.ToString())
            : _userId;

    public string? GetUserEmail() =>
        IsAuthenticated()
            ? _user!.GetEmail()
            : string.Empty;

    public bool IsAuthenticated() =>
        _user?.Identity?.IsAuthenticated is true;

    public bool IsInRole(string role) =>
        _user?.IsInRole(role) is true;

    public IEnumerable<Claim>? GetUserClaims() =>
        _user?.Claims;

    // public string? GetTenant() =>
    //     IsAuthenticated() ? _user?.GetTenant() : string.Empty;

    public void SetCurrentUser(ClaimsPrincipal user)
    {
        _user = user;
    }

    public void SetCurrentUserId(string userId)
    {
        if (!string.IsNullOrEmpty(userId))
        {
            _userId = DefaultIdType.Parse(userId);
        }
    }
}