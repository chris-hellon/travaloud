using Travaloud.Domain.Catalog.Services;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Services.EventHandlers;

public class ServiceCreatedEventHandler : EventNotificationHandler<EntityCreatedEvent<Service>>
{
    private readonly ILogger<ServiceCreatedEventHandler> _logger;

    public ServiceCreatedEventHandler(ILogger<ServiceCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityCreatedEvent<Service> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}