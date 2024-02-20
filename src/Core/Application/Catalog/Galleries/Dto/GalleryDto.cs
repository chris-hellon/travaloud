namespace Travaloud.Application.Catalog.Galleries.Dto;

public class GalleryDto
{
    public DefaultIdType Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }
    public DateTime CreatedOn { get; set; }
}