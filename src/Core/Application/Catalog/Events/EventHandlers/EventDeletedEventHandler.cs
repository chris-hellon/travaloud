using Travaloud.Domain.Catalog.Events;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Events.EventHandlers;

public class EventDeletedEventHandler : EventNotificationHandler<EntityDeletedEvent<Event>>
{
    private readonly ILogger<EventDeletedEventHandler> _logger;

    public EventDeletedEventHandler(ILogger<EventDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityDeletedEvent<Event> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}