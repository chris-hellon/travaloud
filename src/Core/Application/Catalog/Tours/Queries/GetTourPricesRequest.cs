using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Queries;

public class GetTourPricesRequest : IRequest<IEnumerable<TourPriceDto>>
{
    public IEnumerable<DefaultIdType> TourIds { get; set; }

    public GetTourPricesRequest(IEnumerable<DefaultIdType> tourIds)
    {
        TourIds = tourIds;
    }
}

internal class GetTourPricesRequestHandler : IRequestHandler<GetTourPricesRequest, IEnumerable<TourPriceDto>>
{
    private readonly IRepositoryFactory<TourPrice> _repository;

    public GetTourPricesRequestHandler(IRepositoryFactory<TourPrice> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TourPriceDto>> Handle(GetTourPricesRequest request, CancellationToken cancellationToken)
    {
        return await _repository.ListAsync(new TourPricesSpec(request), cancellationToken);
    }
}