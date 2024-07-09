using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class TourPricesSpec : Specification<TourPrice, TourPriceDto>
{
    public TourPricesSpec(GetTourPricesRequest request) =>
        Query.Where(x => request.TourIds.Contains(x.TourId));
}

public class TourPricesByTourIdSpec : Specification<TourPrice, TourPriceDto>
{
    public TourPricesByTourIdSpec(DefaultIdType tourId) =>
        Query.Where(x => x.TourId == tourId);
}