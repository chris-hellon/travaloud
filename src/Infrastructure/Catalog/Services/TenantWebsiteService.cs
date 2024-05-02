using MediatR;
using Microsoft.AspNetCore.Hosting;
using Travaloud.Application.Catalog.Destinations.Dto;
using Travaloud.Application.Catalog.Destinations.Queries;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.PageSorting.Dto;
using Travaloud.Application.Catalog.PageSorting.Queries;
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

    public Task<IEnumerable<PropertyDto>?> GetProperties(CancellationToken cancellationToken)
    {
        return Mediator.Send(new GetPropertiesByPublishToWebsiteRequest(), cancellationToken);
    }

    public Task<IEnumerable<DestinationDto>> GetDestinations(CancellationToken cancellationToken)
    {
        return Mediator.Send(new GetDestinationsRequest(), cancellationToken);
    }
    
    public Task<IEnumerable<TourDto>> GetTours(CancellationToken cancellationToken)
    {
        return Mediator.Send(new GetToursByPublishToWebsiteRequest(), cancellationToken);
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