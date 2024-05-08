using Travaloud.Application.Catalog.Tours.Commands;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Application.Common.Exporters;

namespace Travaloud.Application.Catalog.Interfaces;

public interface IToursService : ITransientService
{
    Task<PaginationResponse<TourDto>?> SearchAsync(SearchToursRequest request);

    Task<TourDetailsDto?> GetAsync(DefaultIdType id);

    Task<IEnumerable<TourCategoryDto>> GetCategoriesAsync();

    Task<IEnumerable<TourCategoryDto>> GetParentCategoriesAsync();

    Task<PaginationResponse<TourDateDto>> GetTourDatesAsync(DefaultIdType tourId, int requestedSpaces);
    
    Task<bool> GetTourDatesByPriceAsync(GetTourDatesByPriceRequest request);
    
    Task<DefaultIdType?> CreateAsync(CreateTourRequest request);

    Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateTourRequest request);

    Task<DefaultIdType?> DeleteAsync(DefaultIdType id);

    Task<FileResponse?> ExportAsync(ExportToursRequest filter);

    Task<IEnumerable<TourWithoutDatesDto>> GetToursWithDetails(GetToursWithDetailsRequest request);

    Task<IEnumerable<TourWithoutDatesDto>> GetToursByDestinations(GetToursByDestinationsRequest request);

    Task<IEnumerable<TourDetailsDto>> GetToursByDestinations(GetToursByDestinationsWithDatesRequest request);

    Task<IEnumerable<TourPickupLocationDto>> GetTourPickupLocations(DefaultIdType id);

    Task<IEnumerable<TourDetailsDto>> SearchToursByDateRangeAndDestinations(SearchToursByDateRangeAndDestinationsRequest request);
}