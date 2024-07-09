using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;


public class TourPickupLocationsByTourIdSpec : Specification<TourPickupLocation, TourPickupLocationDto>
{
    public TourPickupLocationsByTourIdSpec(DefaultIdType tourId) =>
        Query.Where(x => x.TourId == tourId);
}