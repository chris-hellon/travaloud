using Travaloud.Domain.Catalog.Tours;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Tours.EventHandlers;

public class TourCreatedEventHandler : EventNotificationHandler<EntityCreatedEvent<Tour>>
{
    private readonly ILogger<TourCreatedEventHandler> _logger;

    public TourCreatedEventHandler(ILogger<TourCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityCreatedEvent<Tour> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}