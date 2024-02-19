using Travaloud.Domain.Catalog.Destinations;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Destinations.Commands;

public class DeleteDestinationRequest : IRequest<DefaultIdType>
{
    public DeleteDestinationRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class DeleteDestinationRequestHandler : IRequestHandler<DeleteDestinationRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Destination> _repository;
    private readonly IStringLocalizer<DeleteDestinationRequestHandler> _localizer;

    public DeleteDestinationRequestHandler(IRepositoryFactory<Destination> repository,
        IStringLocalizer<DeleteDestinationRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<DefaultIdType> Handle(DeleteDestinationRequest request, CancellationToken cancellationToken)
    {
        var destination = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = destination ?? throw new NotFoundException(_localizer["product.notfound"]);

        // Add Domain Events to be raised after the commit
        destination.DomainEvents.Add(EntityDeletedEvent.WithEntity(destination));

        await _repository.DeleteAsync(destination, cancellationToken);

        return request.Id;
    }
}