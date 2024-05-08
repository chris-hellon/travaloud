using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class ToursByDestinationIdSpec : Specification<Tour, TourWithoutDatesDto>
{
    public ToursByDestinationIdSpec(DefaultIdType id)
    {
        Query.Include(p => p.TourDestinationLookups)
            .ThenInclude(x => x.Destination)
            .Where(p => p.TourDestinationLookups != null &&
                        p.TourDestinationLookups.Any(x => x.DestinationId == id));
    }
}

public class ToursByDestinationsSpec : Specification<Tour, TourWithoutDatesDto>
{
    public ToursByDestinationsSpec(GetToursByDestinationsRequest request) =>
        Query
            .Include(p => p.Images)
            .Include(p => p.TourPrices)
            .Include(p => p.TourDestinationLookups).ThenInclude(x => x.Destination)
            .Include(p => p.TourItineraries)
            .ThenInclude(p => p.Sections)
            .ThenInclude(p => p.Images)
            .Where(p => p.TourDestinationLookups != null && p.TourDestinationLookups.Any(lookup => request.DestinationIds.Contains(lookup.DestinationId)));
}

public class ToursWithDetailsByDestinationsSpec : Specification<Tour, TourDetailsDto>
{
    public ToursWithDetailsByDestinationsSpec(GetToursByDestinationsWithDatesRequest request) =>
        Query
            .Include(p => p.Images)
            .Include(p => p.TourPrices)
            .Include(p => p.TourDates)
            .Include(p => p.TourDestinationLookups).ThenInclude(x => x.Destination)
            .Include(p => p.TourItineraries)
            .ThenInclude(p => p.Sections)
            .ThenInclude(p => p.Images)
            .Where(p => p.TourDestinationLookups != null && p.TourDestinationLookups.Any(lookup => request.DestinationIds.Contains(lookup.DestinationId)));
}