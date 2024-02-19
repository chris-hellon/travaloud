using Travaloud.Domain.Catalog.Services;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Services.EventHandlers;

public class ServiceDeletedEventHandler : EventNotificationHandler<EntityDeletedEvent<Service>>
{
    private readonly ILogger<ServiceDeletedEventHandler> _logger;

    public ServiceDeletedEventHandler(ILogger<ServiceDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityDeletedEvent<Service> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}