using Travaloud.Domain.Catalog.Partners;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Partners.EventHandlers;

public class PartnerDeletedEventHandler : EventNotificationHandler<EntityDeletedEvent<Partner>>
{
    private readonly ILogger<PartnerDeletedEventHandler> _logger;

    public PartnerDeletedEventHandler(ILogger<PartnerDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityDeletedEvent<Partner> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}