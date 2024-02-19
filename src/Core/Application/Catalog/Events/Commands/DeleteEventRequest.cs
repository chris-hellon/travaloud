using Travaloud.Domain.Catalog.Events;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Events.Commands;

public class DeleteEventRequest : IRequest<DefaultIdType>
{
    public DeleteEventRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class DeleteEventRequestHandler : IRequestHandler<DeleteEventRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Event> _repository;
    private readonly IStringLocalizer<DeleteEventRequestHandler> _localizer;

    public DeleteEventRequestHandler(IRepositoryFactory<Event> repository,
        IStringLocalizer<DeleteEventRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<DefaultIdType> Handle(DeleteEventRequest request, CancellationToken cancellationToken)
    {
            var booking = await _repository.GetByIdAsync(request.Id, cancellationToken);

            _ = booking ?? throw new NotFoundException(_localizer["event.notfound"]);

            // Add Domain Events to be raised after the commit
            booking.DomainEvents.Add(EntityDeletedEvent.WithEntity(booking));

            await _repository.DeleteAsync(booking, cancellationToken);

            return request.Id;
        }
}