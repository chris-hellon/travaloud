using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class TourByIdSpec : Specification<Tour>, ISingleResultSpecification<Tour>
{
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
    public TourByIdSpec(DefaultIdType tourId) =>
        Query.Where(p => p.Id == tourId)
            .Include(p => p.TourPrices)
            .Include(p => p.TourCategory)
            .Include(p => p.TourDates)
            .Include(p => p.TourPickupLocations)
            .Include(p => p.TourCategoryLookups)
            .Include(p => p.Images)
            .AsSplitQuery();
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
}