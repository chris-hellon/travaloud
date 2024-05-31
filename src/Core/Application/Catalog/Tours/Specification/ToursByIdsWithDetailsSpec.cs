using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class ToursByIdsWithDetailsSpec : Specification<Tour, TourWithoutDatesDto>
{
    public ToursByIdsWithDetailsSpec(GetToursWithDetailsRequest request) =>
        Query
            .Include(p => p.Images)
            .Include(p => p.TourCategory)
            .Include(p => p.TourPrices)
            .Include(p => p.TourDestinationLookups).ThenInclude(x => x.Destination)
            .Include(p => p.TourItineraries)
            .ThenInclude(p => p.Sections)
            .ThenInclude(p => p.Images)
            .Where(p => request.TourIds.Contains(p.Id));
}