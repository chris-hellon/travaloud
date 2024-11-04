using Travaloud.Application.Catalog.Destinations.Dto;
using Travaloud.Application.Catalog.PageSorting.Dto;
using Travaloud.Application.Catalog.PageSorting.Queries;
using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Application.Catalog.Seo;
using Travaloud.Application.Catalog.Services.Dto;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;

namespace Travaloud.Application.Catalog.Interfaces;

public interface ITenantWebsiteService : ITransientService
{
    Task<IEnumerable<PropertyDto>> GetProperties(CancellationToken cancellationToken);
    
    Task<IEnumerable<TourDto>> GetTours(CancellationToken cancellationToken);

    Task<IEnumerable<TourPriceDto>> GetTourPrices(GetTourPricesRequest request, CancellationToken cancellationToken);
    
    Task<IEnumerable<ServiceDto>> GetServices(CancellationToken cancellationToken);

    Task<IEnumerable<DestinationDto>> GetDestinations(CancellationToken cancellationToken);

    Task<IEnumerable<TourWithCategoryDto>> GetToursWithCategories(string tenantId, CancellationToken cancellationToken);

    Task<IEnumerable<TourWithCategoryDto>> GetTopLevelToursWithCategories(string tenantId, CancellationToken cancellationToken);

    Task<IEnumerable<PageSortingDto>> GetPageSortings(GetPageSortingsRequest request);

    Task<List<SeoRedirectDto>> GetSeoRedirects(CancellationToken cancellationToken);
}