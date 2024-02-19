using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Bookings.EventHandlers;

public class BookingDeletedEventHandler : EventNotificationHandler<EntityDeletedEvent<Booking>>
{
    private readonly ILogger<BookingDeletedEventHandler> _logger;

    public BookingDeletedEventHandler(ILogger<BookingDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityDeletedEvent<Booking> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}