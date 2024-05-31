using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.TourDates.Specification;

public class TourDatesByPriceSpec : Specification<TourDate, TourDateDto>, ISingleResultSpecification<TourDate>
{
    public TourDatesByPriceSpec(DefaultIdType priceId) =>
        Query.Where(p => p.TourPriceId == priceId);
}