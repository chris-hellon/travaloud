using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class TourImagesByIdSpec : Specification<TourImage, TourImageDto>, ISingleResultSpecification<TourImage>
{
    public TourImagesByIdSpec(DefaultIdType id) =>
        Query.Where(p => p.Id == id);
}