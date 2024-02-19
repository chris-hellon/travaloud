using Travaloud.Domain.Catalog.Partners;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Partners.EventHandlers;

public class PartnerCreatedEventHandler : EventNotificationHandler<EntityCreatedEvent<Partner>>
{
    private readonly ILogger<PartnerCreatedEventHandler> _logger;

    public PartnerCreatedEventHandler(ILogger<PartnerCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityCreatedEvent<Partner> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}