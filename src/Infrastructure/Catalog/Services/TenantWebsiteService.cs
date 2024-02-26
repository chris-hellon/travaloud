using MediatR;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Application.Catalog.Properties.Queries;
using Travaloud.Application.Catalog.Services.Dto;
using Travaloud.Application.Catalog.Services.Queries;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Application.Common.Caching;

namespace Travaloud.Infrastructure.Catalog.Services;

public class TenantWebsiteService : BaseService, ITenantWebsiteService
{
    private readonly ICacheService _cache;
    private readonly ICacheKeyService _cacheKeys;
    
    public TenantWebsiteService(ISender mediator, ICacheService cache, ICacheKeyService cacheKeys) : base(mediator)
    {
        _cache = cache;
        _cacheKeys = cacheKeys;
    }

    public async Task<IEnumerable<PropertyDto>?> GetProperties(CancellationToken cancellationToken)
    {
        var currentDate = DateTime.Now.ToShortDateString();
        return await _cache.GetOrSetAsync(
            _cacheKeys.GetCacheKey("Properties", currentDate),
            () => Mediator.Send(new GetPropertiesByPublishToWebsiteRequest(), cancellationToken
            ), cancellationToken: cancellationToken);
    }
    
    public async Task<IEnumerable<TourDto>?> GetTours(CancellationToken cancellationToken)
    {
        var currentDate = DateTime.Now.ToShortDateString();
        return await _cache.GetOrSetAsync(
            _cacheKeys.GetCacheKey("Tours", currentDate),
            () => Mediator.Send(new GetToursByPublishToWebsiteRequest(), cancellationToken
            ), cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<ServiceDto>?> GetServices(CancellationToken cancellationToken)
    {
        var currentDate = DateTime.Now.ToShortDateString();
        return await _cache.GetOrSetAsync(
            _cacheKeys.GetCacheKey("Services", currentDate),
            () => Mediator.Send(new GetServicesRequest(), cancellationToken
            ), cancellationToken: cancellationToken);
    }
}