using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class TourItineraryByIdSpec : Specification<TourItinerary, TourItineraryDto>, ISingleResultSpecification<TourItinerary>
{
    public TourItineraryByIdSpec(DefaultIdType tourId) =>
        Query.Where(p => p.Id == tourId).Include(x => x.Sections).ThenInclude(i => i.Images);
}