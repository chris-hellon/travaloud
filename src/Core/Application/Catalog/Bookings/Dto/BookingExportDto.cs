using System.ComponentModel.DataAnnotations;
using Travaloud.Application.Catalog.Tours.Dto;

namespace Travaloud.Application.Catalog.Bookings.Dto;

public class BookingExportDto
{
    [ExportColumn("Reference")]
    public int BookingInvoiceId { get; set; }

    [ExportColumn("Booking Date")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
    public DateTime BookingBookingDate { get; set; }
    
    [ExportColumn(true)]
    public DefaultIdType? TourId { get; set; }

    [ExportColumn("Tour")]
    public string? TourName { get; set; }
    
    // [ExportColumn(true)]
    // public string? TourCategoryName { get; set; }
    
    [ExportColumn("Pickup Location")]
    public string? PickupLocation { get; set; }
    
    [ExportColumn("Tour Start Date")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
    public DateTime StartDate { get; set; }

    [ExportColumn("Tour End Date")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
    public DateTime EndDate { get; set; }

    [ExportColumn("Amount")]
    public string AmountFormatted => $"${string.Format("{0:n2}", Amount)}";
    
    [ExportColumn(true)]
    public decimal Amount { get; set; }
    
    [ExportColumn("Price Label")]
    public string PriceLabel { get; set; }
    
    [ExportColumn(true)]
    public decimal? BookingAmountOutstanding { get; set; }
    
    [ExportColumn(true)]
    public string BookingCurrencyCode { get; set; }

    [ExportColumn("Status")]
    public string Status => Cancelled.HasValue && Cancelled.Value ? "Cancelled" : NoShow.HasValue && NoShow.Value ? "No Show" : CheckedIn.HasValue && CheckedIn.Value ? "Checked In" : BookingIsPaid ? "Paid" :
        (BookingRefunded.HasValue && BookingRefunded.Value) ? "Refunded" : "Unpaid";
    
    [ExportColumn(true)]
    public bool? BookingRefunded { get; set; }
        
    [ExportColumn(true)]
    public bool BookingIsPaid { get; set; }

    [ExportColumn(true)]
    public string? BookingGuestId { get; set; }
    
    [ExportColumn(true)]
    public string? BookingBookedBy { get; set; }

    [ExportColumn(true)]
    public string? GuestId { get; set; }
    
    [ExportColumn("Guest Name")]
    public string? GuestName { get; set; }

    [ExportColumn("Date of Birth")]
    [DataType(DataType.Date)]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
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
    public bool WaiverSigned { get; set; }
    
    [ExportColumn("Booking Source")]
    public string? BookingBookingSource { get; set; }
    
    [ExportColumn("Booked By")]
    public string? BookingStaffName { get; set; }
    
    [ExportColumn(true)]
    public IList<BookingItemGuestDto>? Guests { get; set; }
    
    [ExportColumn(true)]
    public DefaultIdType CreatedBy { get; set; }
    
    [ExportColumn(true)]
    public bool? Cancelled { get; set; }
    
    [ExportColumn(true)]
    public bool? NoShow { get; set; }
    
    [ExportColumn(true)]
    public bool? CheckedIn { get; set; }
    
    [ExportColumn(true)]
    public DefaultIdType TourDateId { get; set; }
}