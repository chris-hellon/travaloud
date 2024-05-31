namespace Travaloud.Domain.Catalog.Bookings;

public class BookingPayment : AuditableEntity, IAggregateRoot
{
    public DefaultIdType BookingId { get; set; }
    public decimal Amount { get; set; }
    [MaxLength(450)] public string StripeSessionId { get; set; } = default!;
    public bool Refunded { get; set; }

    public virtual Booking Booking { get; set; } = default!;
}