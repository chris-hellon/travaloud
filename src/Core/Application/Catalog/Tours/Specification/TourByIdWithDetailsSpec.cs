using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Domain.Catalog.Tours;

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

namespace Travaloud.Application.Catalog.Tours.Specification;

public class TourByIdWithDetailsSpec : Specification<Tour, TourWithoutDatesDto>, ISingleResultSpecification<Tour>
{

    public TourByIdWithDetailsSpec(DefaultIdType id) =>
        Query
            .Include(p => p.Images)
            .Include(p => p.TourPrices)
            .Include(p => p.TourPickupLocations)
            .Include(p => p.TourDestinationLookups).ThenInclude(x => x.Destination)
            .Include(p => p.TourItineraries)
            .ThenInclude(p => p.Sections)
            .ThenInclude(p => p.Images)
            .Where(p => p.Id == id);
}