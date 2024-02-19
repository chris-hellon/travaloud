using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Bookings.EventHandlers;

public class BookingItemDeletedEventHandler : EventNotificationHandler<EntityDeletedEvent<BookingItem>>
{
    private readonly ILogger<BookingItemDeletedEventHandler> _logger;

    public BookingItemDeletedEventHandler(ILogger<BookingItemDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityDeletedEvent<BookingItem> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}