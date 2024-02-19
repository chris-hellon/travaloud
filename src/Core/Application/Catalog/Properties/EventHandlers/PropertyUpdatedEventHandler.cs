using Travaloud.Domain.Catalog.Properties;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Properties.EventHandlers;

public class PropertyUpdatedEventHandler : EventNotificationHandler<EntityUpdatedEvent<Property>>
{
    private readonly ILogger<PropertyUpdatedEventHandler> _logger;

    public PropertyUpdatedEventHandler(ILogger<PropertyUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityUpdatedEvent<Property> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}