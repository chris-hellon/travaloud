using Finbuckle.MultiTenant;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Travaloud.Application.Catalog.Destinations.Dto;
using Travaloud.Application.Catalog.Destinations.Queries;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.PageSorting.Dto;
using Travaloud.Application.Catalog.PageSorting.Queries;
using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Application.Catalog.Properties.Queries;
using Travaloud.Application.Catalog.Seo;
using Travaloud.Application.Catalog.Services.Dto;
using Travaloud.Application.Catalog.Services.Queries;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Application.Common.Caching;
using Travaloud.Infrastructure.Multitenancy;

namespace Travaloud.Infrastructure.Catalog.Services;

public class TenantWebsiteService : BaseService, ITenantWebsiteService
{
    private readonly ICacheService _cache;
    private readonly ICacheKeyService _cacheKeys;
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly string _tenantKey;
    
    public TenantWebsiteService(ISender mediator,
        ICacheService cache,
        ICacheKeyService cacheKeys,
        IWebHostEnvironment hostEnvironment,
        IMultiTenantContextAccessor<TravaloudTenantInfo> multiTenantContextAccessor) : base(mediator)
    {
        _cache = cache;
        _cacheKeys = cacheKeys;
        _hostEnvironment = hostEnvironment;
        _tenantKey = multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Identifier ?? string.Empty;
    }

    public Task<IEnumerable<PropertyDto>> GetProperties(CancellationToken cancellationToken)
    {
        // if (_hostEnvironment.IsDevelopment())
        //     return Mediator.Send(new GetPropertiesByPublishToWebsiteRequest(), cancellationToken);
        
        var currentDate = DateTime.Now;
        var cacheKey = _cacheKeys.GetCacheKey($"{_tenantKey}-Properties", $"{currentDate:yyyy-MM-dd-HH}");
        
        return _cache.GetOrSetAsync(
            cacheKey,
            () => Mediator.Send(new GetPropertiesByPublishToWebsiteRequest(), cancellationToken),
            cancellationToken: cancellationToken);
    }
    
    public Task<List<SeoRedirectDto>> GetSeoRedirects(CancellationToken cancellationToken)
    {
        var currentDate = DateTime.Now;
        var cacheKey = _cacheKeys.GetCacheKey($"{_tenantKey}-SeoRedirects", $"{currentDate:yyyy-MM-dd-HH}");
        
        return _cache.GetOrSetAsync(
            cacheKey,
            () => Mediator.Send(new GetSeoRedirectsFullRequest(), cancellationToken),
            cancellationToken: cancellationToken);
    }

    public Task<IEnumerable<DestinationDto>> GetDestinations(CancellationToken cancellationToken)
    {
        // if (_hostEnvironment.IsDevelopment())
        //     return Mediator.Send(new GetDestinationsRequest(), cancellationToken);

        var currentDate = DateTime.Now;
        var cacheKey = _cacheKeys.GetCacheKey($"{_tenantKey}-Destinations", $"{currentDate:yyyy-MM-dd-HH}");

        return _cache.GetOrSetAsync(
            cacheKey,
            () => Mediator.Send(new GetDestinationsRequest(), cancellationToken),
            cancellationToken: cancellationToken);
    }
    
    public Task<IEnumerable<TourDto>> GetTours(CancellationToken cancellationToken)
    {
        // if (_hostEnvironment.IsDevelopment())
        //     return Mediator.Send(new GetToursByPublishToWebsiteRequest(), cancellationToken);

        var currentDate = DateTime.Now;
        var cacheKey = _cacheKeys.GetCacheKey($"{_tenantKey}-Tours", $"{currentDate:yyyy-MM-dd-HH}");

        return _cache.GetOrSetAsync(
            cacheKey,
            () => Mediator.Send(new GetToursByPublishToWebsiteRequest(), cancellationToken),
            cancellationToken: cancellationToken);
    }

    public Task<IEnumerable<TourPriceDto>> GetTourPrices(GetTourPricesRequest request, CancellationToken cancellationToken)
    {
        return Mediator.Send(request, cancellationToken);
    }
    
    public Task<IEnumerable<ServiceDto>> GetServices(CancellationToken cancellationToken)
    {
        return Mediator.Send(new GetServicesRequest(), cancellationToken);
    }

    public Task<IEnumerable<TourWithCategoryDto>> GetToursWithCategories(string tenantId, CancellationToken cancellationToken)
    {
        return Mediator.Send(new GetToursWithCategoriesRequest(tenantId), cancellationToken);
    }
    
    public Task<IEnumerable<TourWithCategoryDto>> GetTopLevelToursWithCategories(string tenantId, CancellationToken cancellationToken)
    {
        return Mediator.Send(new GetTopLevelToursWithCategoriesRequest(tenantId), cancellationToken);
    }

    public Task<IEnumerable<PageSortingDto>> GetPageSortings(GetPageSortingsRequest request)
    {
        return Mediator.Send(request);
    }
}