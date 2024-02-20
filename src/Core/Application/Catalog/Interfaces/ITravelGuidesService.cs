using Travaloud.Application.Catalog.TravelGuides.Commands;
using Travaloud.Application.Catalog.TravelGuides.Dto;
using Travaloud.Application.Catalog.TravelGuides.Queries;

namespace Travaloud.Application.Catalog.Interfaces;

public interface ITravelGuidesService : ITransientService
{
    Task<PaginationResponse<TravelGuideDto>> SearchAsync(SearchTravelGuidesRequest request);

    Task<TravelGuideDto> GetAsync(DefaultIdType id);

    Task<DefaultIdType> CreateAsync(CreateTravelGuideRequest request);

    Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateTravelGuideRequest request);

    Task<DefaultIdType> DeleteAsync(DefaultIdType id);
}