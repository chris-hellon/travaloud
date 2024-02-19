using Travaloud.Domain.Common.Events;
using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Application.Catalog.Properties.EventHandlers;

public class PropertyCreatedEventHandler : EventNotificationHandler<EntityCreatedEvent<Property>>
{
    private readonly ILogger<PropertyCreatedEventHandler> _logger;

    public PropertyCreatedEventHandler(ILogger<PropertyCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityCreatedEvent<Property> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}