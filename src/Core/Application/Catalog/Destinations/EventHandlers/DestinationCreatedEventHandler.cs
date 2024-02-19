using Travaloud.Domain.Catalog.Destinations;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Destinations.EventHandlers;

public class DestinationCreatedEventHandler : EventNotificationHandler<EntityCreatedEvent<Destination>>
{
    private readonly ILogger<DestinationCreatedEventHandler> _logger;

    public DestinationCreatedEventHandler(ILogger<DestinationCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityCreatedEvent<Destination> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}