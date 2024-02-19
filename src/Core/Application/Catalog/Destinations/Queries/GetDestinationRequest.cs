using Travaloud.Application.Catalog.Destinations.Dto;
using Travaloud.Application.Catalog.Destinations.Specification;
using Travaloud.Domain.Catalog.Destinations;

namespace Travaloud.Application.Catalog.Destinations.Queries;

public class GetDestinationRequest : IRequest<DestinationDetailsDto>
{
    public GetDestinationRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class GetDestinationRequestHandler : IRequestHandler<GetDestinationRequest, DestinationDetailsDto>
{
    private readonly IRepositoryFactory<Destination> _repository;
    private readonly IStringLocalizer<GetDestinationRequestHandler> _localizer;

    public GetDestinationRequestHandler(IRepositoryFactory<Destination> repository,
        IStringLocalizer<GetDestinationRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<DestinationDetailsDto> Handle(GetDestinationRequest request, CancellationToken cancellationToken) =>
        await _repository.FirstOrDefaultAsync(
            new DestinationByIdWithToursSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(_localizer["destination.notfound"], request.Id));
}