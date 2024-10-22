namespace Travaloud.Application.Catalog.Tours.Dto;

public class TourWithCategoryDto
{
    public DefaultIdType Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public string? ImagePath { get; set; } = default!;
    public string? ThumbnailImagePath { get; set; } = default!;
    public DefaultIdType? TourCategoryId { get; set; }
    public string FriendlyUrl => Name.UrlFriendly();
    public bool IsCategory { get; set; } = default!;
    public bool IsDestination { get; set; } = default!;
    public DefaultIdType? ParentTourCategoryId { get; set; }
    public IEnumerable<TourPriceDto>? TourPrices { get; set; }
    public IEnumerable<TourWithCategoryDto>? ChildTours { get; set; }
    public string? DayDuration { get; set; }
    public string? NightDuration { get; set; }
    public string? HourDuration { get; set; }
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }
    public DefaultIdType? GroupParentCategoryId { get; set; }
    public int? SortOrder { get; set; }
    public bool? WaiverRequired { get; set; }
    public bool? ShowBookingQRCode { get; set; }
}