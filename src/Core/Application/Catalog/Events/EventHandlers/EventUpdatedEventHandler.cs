using Travaloud.Domain.Catalog.Events;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Events.EventHandlers;

public class EventUpdatedEventHandler : EventNotificationHandler<EntityUpdatedEvent<Event>>
{
    private readonly ILogger<EventUpdatedEventHandler> _logger;

    public EventUpdatedEventHandler(ILogger<EventUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityUpdatedEvent<Event> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}