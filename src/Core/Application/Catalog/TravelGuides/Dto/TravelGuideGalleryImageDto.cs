namespace Travaloud.Application.Catalog.TravelGuides.Dto;

public class TravelGuideGalleryImageDto
{
    public DefaultIdType Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? ImagePath { get; set; }
    public int SortOrder { get; set; }
}