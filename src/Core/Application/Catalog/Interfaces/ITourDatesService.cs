using Travaloud.Application.Catalog.TourDates.Queries;
using Travaloud.Application.Catalog.Tours.Dto;

namespace Travaloud.Application.Catalog.Interfaces;

public interface ITourDatesService : ITransientService
{
    Task<PaginationResponse<TourDateDto>> SearchAsync(SearchTourDatesRequest request);
}