using Travaloud.Domain.Catalog.Partners;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Partners.EventHandlers;

public class PartnerUpdatedEventHandler : EventNotificationHandler<EntityUpdatedEvent<Partner>>
{
    private readonly ILogger<PartnerUpdatedEventHandler> _logger;

    public PartnerUpdatedEventHandler(ILogger<PartnerUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityUpdatedEvent<Partner> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}