﻿namespace Travaloud.Application.Catalog.Bookings.Dto;

public class BookingItemDto
{
    public DefaultIdType Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    public int? RoomQuantity { get; set; }
    public DefaultIdType? PropertyId { get; set; }
    public DefaultIdType? TourId { get; set; }
    public string CloudbedsReservationId { get; set; } = default!;
    public int? CloudbedsPropertyId { get; set; }
    public int ConcurrencyVersion { get; set; } = default!;
}