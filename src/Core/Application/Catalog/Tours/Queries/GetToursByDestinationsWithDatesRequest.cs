using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Queries;

public class GetToursByDestinationsWithDatesRequest : IRequest<IEnumerable<TourDetailsDto>>
{   
    public IEnumerable<DefaultIdType> DestinationIds { get; set; }

    public GetToursByDestinationsWithDatesRequest(IEnumerable<DefaultIdType> destinationIds)
    {
        DestinationIds = destinationIds;
    }
}

internal class GetToursByDestinationsWithDatesRequestHandler : IRequestHandler<GetToursByDestinationsWithDatesRequest, IEnumerable<TourDetailsDto>>
{
    private readonly IRepositoryFactory<Tour> _repository;

    public GetToursByDestinationsWithDatesRequestHandler(IRepositoryFactory<Tour> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TourDetailsDto>> Handle(GetToursByDestinationsWithDatesRequest request, CancellationToken cancellationToken)
    {
        return await _repository.ListAsync(new ToursWithDetailsByDestinationsSpec(request), cancellationToken);
    }
}