namespace Travaloud.Application.Catalog.Bookings.Dto;

public class StaffBookingDto
{
    [ExportColumn("Staff Name")]
    public string? FullName { get; set; }
    
    [ExportColumn("Total Bookings Made")]
    public int BookingsMade { get; set; }
    
    [ExportColumn("Total Items Booked")]
    public int ItemsCount { get; set; }
    
    [ExportColumn("Total Bookings Revenue")]
    public decimal TotalBookingsAmount { get; set; }
    
    [ExportColumn("Total Commission Amount")]
    public decimal TotalComission { get; set; }
    
    [ExportColumn(true)]
    public int TotalCount { get; set; }
}