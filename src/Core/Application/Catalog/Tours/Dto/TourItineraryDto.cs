namespace Travaloud.Application.Catalog.Tours.Dto;

public class TourItineraryDto
{
    public DefaultIdType Id { get; set; }
    public string Header { get; set; } = default!;
    public DefaultIdType TourId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? SortOrder { get; set; }

    public IList<TourItinerarySectionDto> Sections { get; set; } = default!;
}