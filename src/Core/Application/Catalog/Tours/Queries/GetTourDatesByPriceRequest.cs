using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Queries;

public class GetTourDatesByPriceRequest : IRequest<bool>
{
    public DefaultIdType PriceId { get; set; }
}

internal class GetTourDatesByPriceRequestHandler : IRequestHandler<GetTourDatesByPriceRequest, bool>
{
    private readonly IRepositoryFactory<TourDate> _repository;

    public GetTourDatesByPriceRequestHandler(IRepositoryFactory<TourDate> repository)
    {
        _repository = repository;
    }


    public async Task<bool> Handle(GetTourDatesByPriceRequest request, CancellationToken cancellationToken)
    {
        var tourDates = await _repository.SingleOrDefaultAsync(new TourDatesByPriceSpec(request.PriceId), cancellationToken);

        return tourDates != null;
    }
}