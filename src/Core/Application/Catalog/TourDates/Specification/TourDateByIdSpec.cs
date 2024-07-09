using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.TourDates.Specification;

public class TourDateByIdSpec : Specification<TourDate>, ISingleResultSpecification<TourDate>
{
    public TourDateByIdSpec(DefaultIdType tourId) =>
        Query
            .Include(x => x.TourPrice)
            .Where(p => p.Id == tourId);
}

public class TourDateWithoutPriceByIdSpec : Specification<TourDate, TourDateDto>, ISingleResultSpecification<TourDate>
{
    public TourDateWithoutPriceByIdSpec(DefaultIdType tourId) =>
        Query
            .Where(p => p.Id == tourId);
}