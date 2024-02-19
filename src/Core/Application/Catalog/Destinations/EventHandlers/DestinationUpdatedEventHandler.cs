using Travaloud.Domain.Catalog.Destinations;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Destinations.EventHandlers;

public class DestinationUpdatedEventHandler : EventNotificationHandler<EntityUpdatedEvent<Destination>>
{
    private readonly ILogger<DestinationUpdatedEventHandler> _logger;

    public DestinationUpdatedEventHandler(ILogger<DestinationUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityUpdatedEvent<Destination> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}