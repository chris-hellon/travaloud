using Travaloud.Domain.Catalog.TravelGuides;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.TravelGuides.EventHandlers;

public class TravelGuidCreatedEventHandler : EventNotificationHandler<EntityCreatedEvent<TravelGuide>>
{
    private readonly ILogger<TravelGuidCreatedEventHandler> _logger;

    public TravelGuidCreatedEventHandler(ILogger<TravelGuidCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityCreatedEvent<TravelGuide> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}