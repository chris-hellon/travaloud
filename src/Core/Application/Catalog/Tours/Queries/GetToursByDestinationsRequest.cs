using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Queries;

public class GetToursByDestinationsRequest : IRequest<IEnumerable<TourWithoutDatesDto>>
{   
    public IEnumerable<DefaultIdType> DestinationIds { get; set; }

    public GetToursByDestinationsRequest(IEnumerable<DefaultIdType> destinationIds)
    {
        DestinationIds = destinationIds;
    }
}

internal class GetToursByDestinationsRequestHandler : IRequestHandler<GetToursByDestinationsRequest, IEnumerable<TourWithoutDatesDto>>
{
    private readonly IRepositoryFactory<Tour> _repository;

    public GetToursByDestinationsRequestHandler(IRepositoryFactory<Tour> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TourWithoutDatesDto>> Handle(GetToursByDestinationsRequest request, CancellationToken cancellationToken)
    {
        return await _repository.ListAsync(new ToursByDestinationsSpec(request), cancellationToken);
    }
}