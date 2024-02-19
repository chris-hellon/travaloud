using Travaloud.Domain.Catalog.Events;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Events.EventHandlers;

public class EventCreatedEventHandler : EventNotificationHandler<EntityCreatedEvent<Event>>
{
    private readonly ILogger<EventCreatedEventHandler> _logger;

    public EventCreatedEventHandler(ILogger<EventCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityCreatedEvent<Event> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}