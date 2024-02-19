using Travaloud.Domain.Catalog.Tours;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Tours.EventHandlers;

public class TourUpdatedEventHandler : EventNotificationHandler<EntityUpdatedEvent<Tour>>
{
    private readonly ILogger<TourUpdatedEventHandler> _logger;

    public TourUpdatedEventHandler(ILogger<TourUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityUpdatedEvent<Tour> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}