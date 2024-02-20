using Travaloud.Domain.Catalog.TravelGuides;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.TravelGuides.EventHandlers;

public class TravelGuideDeletedEventHandler : EventNotificationHandler<EntityDeletedEvent<TravelGuide>>
{
    private readonly ILogger<TravelGuideDeletedEventHandler> _logger;

    public TravelGuideDeletedEventHandler(ILogger<TravelGuideDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityDeletedEvent<TravelGuide> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}