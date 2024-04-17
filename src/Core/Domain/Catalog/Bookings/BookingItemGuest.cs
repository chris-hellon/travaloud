namespace Travaloud.Domain.Catalog.Bookings;

public class BookingItemGuest : AuditableEntity, IAggregateRoot
{
    public DefaultIdType BookingItemId { get; private set; } = default!;
    public DefaultIdType GuestId { get; private set; } = default!;

    public virtual BookingItem BookingItem { get; private set; } = default!;

    public BookingItemGuest(DefaultIdType guestId)
    {
        GuestId = guestId;
    }
}