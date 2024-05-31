namespace Travaloud.Application.Catalog.Bookings.Dto;

public class BookingDetailsDto
{
    public DefaultIdType Id { get; set; }
    public string Description { get; set; } = default!;
    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; } = default!;
    public int ItemQuantity { get; set; }
    public bool IsPaid { get; set; }
    public DateTime BookingDate { get; set; }
    public string? GuestId { get; set; }
    public string? GuestName { get; set; }
    public string? GuestEmail { get; set; }
    public int InvoiceId { get; set; }
    public int ConcurrencyVersion { get; set; } = default!;
    public string? StripeSessionId { get; set; }
    public string? AdditionalNotes { get; set; }
    public bool? WaiverSigned { get; set; }
    public string? BookingSource { get; set; }
    public bool? ConfirmationEmailSent { get; set; }
    public bool? Refunded { get; set; }
    public decimal? AmountOutstanding { get; set; }
    public DefaultIdType CreatedBy { get; set; }
    public IList<BookingItemDetailsDto>? Items { get; set; }
}