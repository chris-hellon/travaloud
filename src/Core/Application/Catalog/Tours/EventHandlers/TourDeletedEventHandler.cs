using Travaloud.Domain.Catalog.Tours;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Tours.EventHandlers;

public class TourDeletedEventHandler : EventNotificationHandler<EntityDeletedEvent<Tour>>
{
    private readonly ILogger<TourDeletedEventHandler> _logger;

    public TourDeletedEventHandler(ILogger<TourDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityDeletedEvent<Tour> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}