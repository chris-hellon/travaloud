namespace Travaloud.Application.Catalog.Galleries.Dto;

public class GalleryDetailsDto
{
    public DefaultIdType Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }

    public virtual IEnumerable<GalleryImageDto> GalleryImages { get; set; } = default!;
}