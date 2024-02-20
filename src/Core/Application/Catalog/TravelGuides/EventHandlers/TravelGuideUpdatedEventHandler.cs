using Travaloud.Domain.Catalog.TravelGuides;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.TravelGuides.EventHandlers;

public class TravelGuideUpdatedEventHandler : EventNotificationHandler<EntityUpdatedEvent<TravelGuide>>
{
    private readonly ILogger<TravelGuideUpdatedEventHandler> _logger;

    public TravelGuideUpdatedEventHandler(ILogger<TravelGuideUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityUpdatedEvent<TravelGuide> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}