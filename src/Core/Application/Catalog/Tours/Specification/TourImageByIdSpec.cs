using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class TourImageByIdSpec : Specification<TourImage>, ISingleResultSpecification<TourImage>
{

    public TourImageByIdSpec(DefaultIdType tourImageId) =>
        Query.Where(p => p.Id == tourImageId);
}