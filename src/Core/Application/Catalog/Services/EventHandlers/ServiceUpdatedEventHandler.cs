using Travaloud.Domain.Catalog.Services;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Services.EventHandlers;

public class ServiceUpdatedEventHandler : EventNotificationHandler<EntityUpdatedEvent<Service>>
{
    private readonly ILogger<ServiceUpdatedEventHandler> _logger;

    public ServiceUpdatedEventHandler(ILogger<ServiceUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityUpdatedEvent<Service> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}