using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class TourDateByIdSpec : Specification<TourDate>, ISingleResultSpecification<TourDate>
{
    public TourDateByIdSpec(DefaultIdType tourId) =>
        Query.Where(p => p.Id == tourId);
}