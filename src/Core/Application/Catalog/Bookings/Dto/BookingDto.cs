﻿namespace Travaloud.Application.Catalog.Bookings.Dto;

public class BookingDto
{
    public DefaultIdType Id { get; set; }
    public string Description { get; set; } = default!;
    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; } = default!;
    public int ItemQuantity { get; set; }
    public bool IsPaid { get; set; }
    public DateTime BookingDate { get; set; }
    public string? GuestId { get; set; }
    public int InvoiceId { get; set; }
    public bool ShowDetails { get; set; } = default!;
    public int ConcurrencyVersion { get; set; } = default!;

    public IList<BookingItemDetailsDto>? Items { get; set; }
}