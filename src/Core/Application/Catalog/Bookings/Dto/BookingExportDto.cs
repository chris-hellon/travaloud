namespace Travaloud.Application.Catalog.Bookings.Dto;

public class BookingExportDto
{
    [ExportColumn("Invoice Id", true)]
    public int BookingInvoiceId { get; set; }

    [ExportColumn("Booking Date")]
    public DateTime BookingBookingDate { get; set; }

    [ExportColumn("Is Paid")]
    public bool BookingIsPaid { get; set; }

    [ExportColumn("Tour Start Date")]
    public DateTime StartDate { get; set; }

    [ExportColumn("Tour End Date")]
    public DateTime EndDate { get; set; }

    [ExportColumn("Amount")]
    public decimal Amount { get; set; }

    [ExportColumn(true)]
    public DefaultIdType? TourId { get; set; }

    [ExportColumn("Tour")]
    public string? TourName { get; set; }

    [ExportColumn(true)]
    public string? BookingGuestId { get; set; }

    [ExportColumn("Guest Name")]
    public string? GuestName { get; set; }

    [ExportColumn("Date of Birth")]
    public DateTime? GuestDateOfBirth { get; set; }

    [ExportColumn("Nationality")]
    public string? GuestNationality { get; set; }

    [ExportColumn("Gender")]
    public string? GuestGender { get; set; }

    [ExportColumn("Passport Number")]
    public string? GuestPassportNumber { get; set; }
}