using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Travaloud.Application.Catalog.Destinations.Dto;
using Travaloud.Application.Catalog.Destinations.Queries;
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
    private readonly IWebHostEnvironment _hostEnvironment;
    
    public TenantWebsiteService(ISender mediator, ICacheService cache, ICacheKeyService cacheKeys, IWebHostEnvironment hostEnvironment) : base(mediator)
    {
        _cache = cache;
        _cacheKeys = cacheKeys;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<IEnumerable<PropertyDto>?> GetProperties(CancellationToken cancellationToken)
    {
        if (_hostEnvironment.IsDevelopment())
            return await Mediator.Send(new GetPropertiesByPublishToWebsiteRequest(), cancellationToken);
        
        var currentDate = DateTime.Now.ToShortDateString();
        return await _cache.GetOrSetAsync(
            _cacheKeys.GetCacheKey("Properties", currentDate),
            () => Mediator.Send(new GetPropertiesByPublishToWebsiteRequest(), cancellationToken
            ), cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<DestinationDto>?> GetDestinations(CancellationToken cancellationToken)
    {
        if (_hostEnvironment.IsDevelopment())
            return await Mediator.Send(new GetDestinationsRequest(), cancellationToken);
        
        var currentDate = DateTime.Now.ToShortDateString();
        return await _cache.GetOrSetAsync(
            _cacheKeys.GetCacheKey("Destinations", currentDate),
            () => Mediator.Send(new GetDestinationsRequest(), cancellationToken
            ), cancellationToken: cancellationToken);
    }
    
    public async Task<IEnumerable<TourDto>?> GetTours(CancellationToken cancellationToken)
    {
        if (_hostEnvironment.IsDevelopment())
            return await Mediator.Send(new GetToursByPublishToWebsiteRequest(), cancellationToken);
        
        var currentDate = DateTime.Now.ToShortDateString();
        return await _cache.GetOrSetAsync(
            _cacheKeys.GetCacheKey("Tours", currentDate),
            () => Mediator.Send(new GetToursByPublishToWebsiteRequest(), cancellationToken
            ), cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<ServiceDto>?> GetServices(CancellationToken cancellationToken)
    {
        if (_hostEnvironment.IsDevelopment())
            return await Mediator.Send(new GetServicesRequest(), cancellationToken);
        
        var currentDate = DateTime.Now.ToShortDateString();
        return await _cache.GetOrSetAsync(
            _cacheKeys.GetCacheKey("Services", currentDate),
            () => Mediator.Send(new GetServicesRequest(), cancellationToken
            ), cancellationToken: cancellationToken);
    }
}