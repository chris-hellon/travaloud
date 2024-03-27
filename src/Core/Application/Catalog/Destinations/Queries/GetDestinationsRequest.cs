using Travaloud.Application.Catalog.Destinations.Dto;
using Travaloud.Application.Catalog.Destinations.Specification;
using Travaloud.Domain.Catalog.Destinations;

namespace Travaloud.Application.Catalog.Destinations.Queries;


public class GetDestinationsRequest : IRequest<IEnumerable<DestinationDto>>
{
    public string? Name { get; set; }
}

public class GetDestinationsRequestHandler : IRequestHandler<GetDestinationsRequest, IEnumerable<DestinationDto>>
{
    private readonly IRepositoryFactory<Destination> _repository;

    public GetDestinationsRequestHandler(IRepositoryFactory<Destination> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<DestinationDto>> Handle(GetDestinationsRequest request, CancellationToken cancellationToken)
    {
        var spec = new GetDestinationsSpec(request);
        return await _repository.ListAsync(spec, cancellationToken: cancellationToken);
    }
}