using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;


public class TourDestinationLookupsByTourIdSpec : Specification<TourDestinationLookup>
{
    public TourDestinationLookupsByTourIdSpec(DefaultIdType tourId) =>
        Query
            .Where(p => p.TourId == tourId);
    // .Where(x => x.AvailableSpaces > 0 && x.AvailableSpaces >= request.RequestedSpaces);
}