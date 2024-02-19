using Travaloud.Domain.Catalog.Destinations;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Destinations.EventHandlers;

public class DestinationDeletedEventHandler : EventNotificationHandler<EntityDeletedEvent<Destination>>
{
    private readonly ILogger<DestinationDeletedEventHandler> _logger;

    public DestinationDeletedEventHandler(ILogger<DestinationDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityDeletedEvent<Destination> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}