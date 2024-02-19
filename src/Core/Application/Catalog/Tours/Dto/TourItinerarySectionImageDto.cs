namespace Travaloud.Application.Catalog.Tours.Dto;

public class TourItinerarySectionImageDto 
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType TourItinerarySectionId { get; set; }
    public string ImagePath { get; set; } = default!;
    public string ThumbnailImagePath { get; set; } = default!;
    public int? SortOrder { get; set; }
}