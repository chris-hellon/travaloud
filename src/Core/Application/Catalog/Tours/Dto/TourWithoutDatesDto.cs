using Travaloud.Application.Catalog.Partners.Dto;

namespace Travaloud.Application.Catalog.Tours.Dto;

public class TourWithoutDatesDto
{
    public DefaultIdType Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public decimal? Price { get; set; }
    public string? PriceLabel { get; set; }
    public string? ImagePath { get; set; }
    public string? ThumbnailImagePath { get; set; }
    public int? MaxCapacity { get; set; }
    public int? MinCapacity { get; set; }
    public string? DayDuration { get; set; }
    public string? NightDuration { get; set; }
    public string? HourDuration { get; set; }
    public string? Address { get; set; }
    public string? TelephoneNumber { get; set; }
    public string? WhatsIncluded { get; set; }
    public string? WhatsNotIncluded { get; set; }
    public string? AdditionalInformation { get; set; }
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }
    public string FriendlyUrl => Name.UrlFriendly();
    public DefaultIdType? TourCategoryId { get; set; }
    public string? SelectedParentTourCategoriesString { get; set; }
    public List<DefaultIdType>? SelectedParentTourCategories { get; set; }
    public bool? PublishToSite { get; set; }
    public string? UrlSlug { get; set; }
    public string? H1 { get; set; }
    public string? H2 { get; set; }
    public string? BookingConfirmationEmailDetails { get; set; }
    public bool? WaiverRequired { get; set; }
    public string? TermsAndConditions { get; set; }
    public DefaultIdType? SupplierId { get; set; }
    public string? SupplierEmailText { get; set; }
    public bool? ShowBookingQRCode { get; set; }
    
    public IList<TourPriceDto>? TourPrices { get; set; }
    public IList<TourItineraryDto>? TourItineraries { get; set; }
    // public IList<TourCategoryLookupDto>? TourCategoryLookups { get; set; }
    public IList<TourCategoryDto>? TourCategories { get; set; }
    public IList<TourCategoryDto>? ParentTourCategories { get; set; }
    public IList<TourImageDto>? Images { get; set; }
    public IList<TourDestinationLookupDto>? TourDestinationLookups { get; set; }
    public IList<TourPickupLocationDto>? TourPickupLocations { get; set; }
    public PartnerDto? Supplier { get; set; }
    public TourCategoryDto? TourCategory { get; set; }
}