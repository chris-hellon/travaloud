namespace Travaloud.Application.Catalog.TravelGuides.Dto;

public class TravelGuideDto
{
    public DefaultIdType Id { get; set; }
    public string Title { get; set; } = default!;
    public string SubTitle { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;
    public string? ImagePath { get; set; }
    public DateTime CreatedOn { get; set; } = default!;
    public string CreatedBy { get; set; } = default!;
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }

    public IEnumerable<TravelGuideGalleryImageDto> TravelGuideGalleryImages { get; set; } = default!;
}