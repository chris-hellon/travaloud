using System.ComponentModel.DataAnnotations.Schema;

namespace Travaloud.Domain.Catalog.Bookings;

public class Booking : AuditableEntity, IAggregateRoot
{
    public string Description { get; private set; } = default!;
    public decimal TotalAmount { get; private set; }
    public string CurrencyCode { get; private set; } = default!;
    public int ItemQuantity { get; private set; }
    public bool IsPaid { get; private set; }
    public DateTime BookingDate { get; private set; }
    public string? GuestId { get; private set; }
    public string? StripeSessionId { get; private set; }
    public bool? BookingConfirmed { get; private set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int InvoiceId { get; private set; }

    public int ConcurrencyVersion { get; set; }
    public virtual IList<BookingItem> Items { get; set; } = default!;

    public Booking()
    {

    }

    public Booking(
        string description,
        decimal totalAmount,
        string currencyCode,
        int itemQuantity,
        bool isPaid,
        DateTime bookingDate,
        string? guestId, 
        string? stripeSessionId)
    {
        Description = description;
        TotalAmount = totalAmount;
        CurrencyCode = currencyCode;
        ItemQuantity = itemQuantity;
        IsPaid = isPaid;
        BookingDate = bookingDate;
        GuestId = guestId;
        StripeSessionId = stripeSessionId;
    }

    public Booking Update(
        string? description = null,
        decimal? totalAmount = null,
        string? currencyCode = null,
        int? itemQuantity = null,
        bool? isPaid = null,
        DateTime? bookingDate = null,
        string? guestId = null,
        string? stripeSessionId = null,
        IList<BookingItem>? items = null)
    {
        if (description is not null && Description != description)
            Description = description;

        if (totalAmount is not null && TotalAmount != totalAmount)
            TotalAmount = totalAmount.Value;

        if (currencyCode is not null && CurrencyCode != currencyCode)
            CurrencyCode = currencyCode;

        if (itemQuantity is not null && ItemQuantity != itemQuantity)
            ItemQuantity = itemQuantity.Value;

        if (isPaid is not null && IsPaid != isPaid)
            IsPaid = isPaid.Value;

        if (bookingDate is not null && BookingDate != bookingDate)
            BookingDate = bookingDate.Value;

        if (guestId is not null && GuestId != guestId)
            GuestId = guestId;
        
        if (items is not null && Items != items)
            Items = items;
        
        if (stripeSessionId is not null && StripeSessionId != stripeSessionId)
            StripeSessionId = stripeSessionId;

        return this;
    }

    public Booking FlagBookingAsPaid()
    {
        IsPaid = true;
        BookingConfirmed = true;

        return this;
    }

    public Booking SetStripeSessonId(string stripeSessionId)
    {
        StripeSessionId = stripeSessionId;
        return this;
    }
}