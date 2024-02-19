using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class TourItineraryByNameSpec : Specification<TourItinerary>, ISingleResultSpecification<TourItinerary>
{
    public TourItineraryByNameSpec(string header, DefaultIdType tourId) =>
        Query.Where(p => p.Header == header && p.TourId == tourId);
}