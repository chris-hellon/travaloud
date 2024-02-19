using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class TourCategoryLookupsByTourCategoryIdsSpec : Specification<TourCategoryLookup, TourCategoryLookupDto>
{
    public TourCategoryLookupsByTourCategoryIdsSpec(IEnumerable<DefaultIdType> tourCategoryIds, DefaultIdType? tourId = null) =>
        Query
            .Where(p =>
                tourCategoryIds.Contains(p.TourCategoryId)
                && (p.TourId == tourId || tourId == null));
}