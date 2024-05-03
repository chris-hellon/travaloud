using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class TourPickupLocationsByTourSpec : Specification<TourPickupLocation, TourPickupLocationDto>
{
    public TourPickupLocationsByTourSpec(DefaultIdType tourId) =>
        Query
            .Where(p => p.TourId == tourId);
}