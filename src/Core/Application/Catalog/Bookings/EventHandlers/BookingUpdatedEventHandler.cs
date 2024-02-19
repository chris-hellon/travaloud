using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Bookings.EventHandlers;

public class BookingUpdatedEventHandler : EventNotificationHandler<EntityUpdatedEvent<Booking>>
{
    private readonly ILogger<BookingUpdatedEventHandler> _logger;

    public BookingUpdatedEventHandler(ILogger<BookingUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityUpdatedEvent<Booking> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}