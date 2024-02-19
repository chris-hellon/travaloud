using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class TourPriceByAmountSpec : Specification<TourPrice>, ISingleResultSpecification<TourPrice>
{
    public TourPriceByAmountSpec(decimal? amount, DefaultIdType tourId) =>
        Query.Where(p => p.Price == amount && p.TourId == tourId);
}