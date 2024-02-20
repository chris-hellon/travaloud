using Travaloud.Domain.Catalog.TravelGuides;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.TravelGuides.EventHandlers;

public class TravelGuideCreatedEventHandler : EventNotificationHandler<EntityCreatedEvent<TravelGuide>>
{
    private readonly ILogger<TravelGuideCreatedEventHandler> _logger;

    public TravelGuideCreatedEventHandler(ILogger<TravelGuideCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityCreatedEvent<TravelGuide> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}