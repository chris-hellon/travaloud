using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class TourCategoryByIdSpec : Specification<TourCategory, TourCategoryDto>, ISingleResultSpecification<TourCategory>
{
    public TourCategoryByIdSpec(DefaultIdType id) =>
        Query.Where(p => p.Id == id);
}