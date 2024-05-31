using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travaloud.Domain.Catalog.Bookings;

public class Booking : AuditableEntity, IAggregateRoot
{
    public string Description { get; private set; } = default!;
    public decimal TotalAmount { get; private set; }
    public decimal? AmountOutstanding { get; private set; }
    public string CurrencyCode { get; private set; } = default!;
    public int ItemQuantity { get; private set; }
    public bool IsPaid { get; private set; }
    public DateTime BookingDate { get; private set; }
    public string? GuestId { get; private set; }
    public string? GuestName { get; private set; }
    public string? GuestEmail { get; private set; }
    public string? StripeSessionId { get; private set; }
    public string? UpdateStripeSessionId { get; private set; }
    public bool? BookingConfirmed { get; private set; }
    public bool? WaiverSigned { get; private set; }
    public string? AdditionalNotes { get; private set; }
    [MaxLength(128)] public string? BookingSource { get; private set; }
    public bool? ConfirmationEmailSent { get; private set; }
    public bool? Refunded { get; private set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int InvoiceId { get; private set; }

    public int ConcurrencyVersion { get; set; }
    
    public virtual IList<BookingItem> Items { get; set; } = default!;
    public virtual IEnumerable<BookingPayment> Payments { get; set; } = default!;
    
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
        string? stripeSessionId,
        bool? waiverSigned,
        string? guestName,
        string? guestEmail,
        string? additionalNotes,
        string? bookingSource,
        bool? refunded, 
        decimal? amountOutstanding)
    {
        Description = description;
        TotalAmount = totalAmount;
        CurrencyCode = currencyCode;
        ItemQuantity = itemQuantity;
        IsPaid = isPaid;
        BookingDate = bookingDate;
        GuestId = guestId;
        StripeSessionId = stripeSessionId;
        WaiverSigned = waiverSigned;
        GuestName = guestName;
        GuestEmail = guestEmail;
        AdditionalNotes = additionalNotes;
        BookingSource = bookingSource;
        Refunded = refunded;
        AmountOutstanding = amountOutstanding;
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
        bool? waiverSigned = null,
        string? guestName = null,
        string? guestEmail = null,
        string? additionalNotes = null,
        string? bookingSource = null,
        bool? refunded = null,
        IList<BookingItem>? items = null,
        DefaultIdType? createdBy = null, 
        bool doNotUpdateAmount = false,
        decimal? amountOutstanding = null,
        string? updateStripeSessionId = null)
    {
        if (description is not null && Description != description)
            Description = description;

        if (!doNotUpdateAmount)
        {
            if (totalAmount is not null && TotalAmount != totalAmount)
                TotalAmount = totalAmount.Value;
            
            if (amountOutstanding is not null && AmountOutstanding != amountOutstanding)
                AmountOutstanding = amountOutstanding.Value;
        }

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
        
        if (updateStripeSessionId is not null && UpdateStripeSessionId != updateStripeSessionId)
            UpdateStripeSessionId = updateStripeSessionId;

        if (waiverSigned is not null && WaiverSigned != waiverSigned)
            WaiverSigned = waiverSigned.Value;

        if (guestName is not null && GuestName != guestName)
            GuestName = guestName;

        if (guestEmail is not null && GuestEmail != guestEmail)
            GuestEmail = guestEmail;

        AdditionalNotes = additionalNotes;
        BookingSource = bookingSource;
        
        if (refunded is not null && Refunded != refunded)
            Refunded = refunded.Value;

        if (!createdBy.HasValue || CreatedBy == createdBy) return this;
        
        OverrideCreatedBy = true;
        CreatedBy = createdBy.Value;

        return this;
    }

    public Booking FlagBookingAsPaid()
    {
        IsPaid = true;
        BookingConfirmed = true;
        AmountOutstanding = 0;

        return this;
    }

    public Booking FlagConfirmationEmailSent()
    {
        ConfirmationEmailSent = true;

        return this;
    }
    
    public Booking FlagBookingAsRefunded()
    {
        Refunded = true;
        IsPaid = false;

        return this;
    }

    public Booking SetStripeSessonId(string stripeSessionId)
    {
        StripeSessionId = stripeSessionId;
        return this;
    }
}