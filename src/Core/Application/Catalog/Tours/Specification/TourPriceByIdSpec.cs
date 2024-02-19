using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class TourPriceByIdSpec : Specification<TourPrice>, ISingleResultSpecification<TourPrice>
{
    public TourPriceByIdSpec(DefaultIdType tourId) =>
        Query.Where(p => p.Id == tourId);
}