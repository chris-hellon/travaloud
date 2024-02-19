using Travaloud.Domain.Catalog.Properties;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Properties.EventHandlers;

public class PropertyDeletedEventHandler : EventNotificationHandler<EntityDeletedEvent<Property>>
{
    private readonly ILogger<PropertyDeletedEventHandler> _logger;

    public PropertyDeletedEventHandler(ILogger<PropertyDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityDeletedEvent<Property> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}