using System.ComponentModel.DataAnnotations;
using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Application.Catalog.Tours.Dto;

namespace Travaloud.Tenants.SharedRCL.Models.WebComponents;

public class BookNowComponent : BaseComponent
{
    public IEnumerable<PropertyDto>? Properties { get; private set; }

    [Required(ErrorMessage = "Please select a Hostel", AllowEmptyStrings = false)]
    public Guid? PropertyId { get; private set; }

    [Required(ErrorMessage = "Please choose a Check In Date")]
    public DateTime? CheckInDate { get; private set; }

    [Required(ErrorMessage = "Please choose a Check In Date")]
    public DateTime? CheckOutDate { get; private set; }
        
    [Required(ErrorMessage = "Please choose a Check In Out Date")]
    public string DateRange { get; private set; }

    [Required(ErrorMessage = "Please choose a Date")]
    public DateTime? TourDate { get; private set; }

    public IEnumerable<TourDto>? Tours { get; private set; } 

    public Guid? TourId { get; private set; }

    [Required(ErrorMessage = "Please choose a Guest Quantity")]
    public int? GuestQuantity { get; private set; }

    public bool Floating { get; private set; }

    public bool IsModal { get; set; }

    public BookNowComponent()
    {

    }

    public BookNowComponent(IEnumerable<TourDto>? tours, Guid? tourId = null, bool floating = false)
    {
        Tours = tours;
        TourId = tourId;
        Floating = floating;
    }

    public BookNowComponent(IEnumerable<PropertyDto>? properties, Guid? propertyId = null, bool floating = false)
    {
        Properties = properties;
        PropertyId = propertyId;
        Floating = floating;
    }

    public BookNowComponent(Guid propertyId, DateTime? checkInDate, DateTime? checkOutDate, bool floating = false, int? guestQuantity = null)
    {
        PropertyId = propertyId;
        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
        GuestQuantity = guestQuantity;
        Floating = floating;
    }

    public BookNowComponent(bool floating = false)
    {
        Floating = floating;
    }
}