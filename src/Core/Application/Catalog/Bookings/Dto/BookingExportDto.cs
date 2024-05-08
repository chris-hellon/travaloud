using System.ComponentModel.DataAnnotations;

namespace Travaloud.Application.Catalog.Bookings.Dto;

public class BookingExportDto
{
    [ExportColumn("Reference")]
    public int BookingInvoiceId { get; set; }

    [ExportColumn("Booking Date")]
    public DateTime BookingBookingDate { get; set; }
    
    [ExportColumn(true)]
    public DefaultIdType? TourId { get; set; }

    [ExportColumn("Tour")]
    public string? TourName { get; set; }
    
    [ExportColumn("Pickup Location")]
    public string? PickupLocation { get; set; }
    
    [ExportColumn("Tour Start Date")]
    public DateTime StartDate { get; set; }

    [ExportColumn("Tour End Date")]
    public DateTime EndDate { get; set; }

    [ExportColumn("Amount")]
    public string AmountFormatted => $"${string.Format("{0:n2}", Amount)}";
    
    [ExportColumn(true)]
    public decimal Amount { get; set; }
    
    [ExportColumn(true)]
    public string BookingCurrencyCode { get; set; }
    
    [ExportColumn("Is Paid")]
    public bool BookingIsPaid { get; set; }

    [ExportColumn(true)]
    public string? BookingGuestId { get; set; }

    [ExportColumn("Guest Name")]
    public string? GuestName { get; set; }

    [ExportColumn("Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime? GuestDateOfBirth { get; set; }

    [ExportColumn("Nationality")]
    public string? GuestNationality { get; set; }

    [ExportColumn("Gender")]
    public string? GuestGender { get; set; }

    [ExportColumn("Passport Number")]
    public string? GuestPassportNumber { get; set; }
    
    [ExportColumn("Additional Notes")]
    public string? BookingAdditionalNotes { get; set; }
    
    [ExportColumn("Waiver Signed")]
    public bool BookingWaiverSigned { get; set; }
    
    [ExportColumn("Booking Source")]
    public string? BookingBookingSource { get; set; }
    
    [ExportColumn("Booked By")]
    public string? BookingStaffName { get; set; }
    
    [ExportColumn(true)]
    public IList<BookingItemGuestDto>? Guests { get; set; }
    
    [ExportColumn(true)]
    public DefaultIdType CreatedBy { get; set; }
}