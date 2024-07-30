namespace Travaloud.Domain.Catalog.Bookings;

public class BookingItemGuest : AuditableEntity, IAggregateRoot
{
    public DefaultIdType BookingItemId { get; private set; } = default!;
    public DefaultIdType GuestId { get; private set; } = default!;
    public bool? Cancelled { get; private set; }
    public bool? NoShow { get; private set; }
    public bool? CheckedIn { get; private set; }

    public virtual BookingItem BookingItem { get; private set; } = default!;

    public BookingItemGuest(DefaultIdType guestId)
    {
        GuestId = guestId;
    }
}