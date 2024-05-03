using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Queries;

public class GetTourPickupLocationsRequest : IRequest<IEnumerable<TourPickupLocationDto>>
{
    public DefaultIdType TourId { get; set; }

    public GetTourPickupLocationsRequest(DefaultIdType tourId)
    {
        TourId = tourId;
    }
}

internal class GetTourPickupLocationsRequestHandler : IRequestHandler<GetTourPickupLocationsRequest, IEnumerable<TourPickupLocationDto>>
{
    private readonly IStringLocalizer<GetTourPickupLocationsRequestHandler> _localizer;
    private readonly IRepositoryFactory<TourPickupLocation> _repository;

    public GetTourPickupLocationsRequestHandler(IRepositoryFactory<TourPickupLocation> repository, IStringLocalizer<GetTourPickupLocationsRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<IEnumerable<TourPickupLocationDto>> Handle(GetTourPickupLocationsRequest request, CancellationToken cancellationToken)
    {
        return await _repository.ListAsync(new TourPickupLocationsByTourSpec(request.TourId), cancellationToken: cancellationToken);
    }
}