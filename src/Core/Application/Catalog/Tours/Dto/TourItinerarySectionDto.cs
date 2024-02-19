namespace Travaloud.Application.Catalog.Tours.Dto;

public class TourItinerarySectionDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType TourItineraryId { get; set; }
    public string? Title { get; set; }
    public string? SubTitle { get; set; }
    public string? Description { get; set; } = default!;
    public string? Highlights { get; set; }
    public int? SortOrder { get; set; }
    
    public virtual IList<TourItinerarySectionImageDto> Images { get; set; } = default!;
}