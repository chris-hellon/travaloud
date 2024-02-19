using Finbuckle.MultiTenant;
using Travaloud.Application.Common.Caching;
using Travaloud.Infrastructure.Multitenancy;

namespace Travaloud.Infrastructure.Caching;

public class CacheKeyService : ICacheKeyService
{
    private ITenantInfo? _currentTenant { get; set; }
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CacheKeyService(ITenantInfo currentTenant, IHttpContextAccessor httpContextAccessor) => (_currentTenant, _httpContextAccessor) = (currentTenant, httpContextAccessor);

    public string GetCacheKey(string name, object id, bool includeTenantId = true)
    {
        if (_httpContextAccessor.HttpContext != null)
            _currentTenant ??= _httpContextAccessor.HttpContext.GetMultiTenantContext<TravaloudTenantInfo>()
                ?.TenantInfo;

        var tenantId = includeTenantId
            ? _currentTenant?.Id ?? throw new InvalidOperationException("GetCacheKey: includeTenantId set to true and no ITenantInfo available.")
            : "GLOBAL";
        return $"{tenantId}-{name}-{id}";
    }
}