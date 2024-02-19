using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Bookings.EventHandlers;

public class BookingCreatedEventHandler : EventNotificationHandler<EntityCreatedEvent<Booking>>
{
    private readonly ILogger<BookingCreatedEventHandler> _logger;

    public BookingCreatedEventHandler(ILogger<BookingCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityCreatedEvent<Booking> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}