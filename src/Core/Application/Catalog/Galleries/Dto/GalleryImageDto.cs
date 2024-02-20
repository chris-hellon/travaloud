namespace Travaloud.Application.Catalog.Galleries.Dto;

public class GalleryImageDto
{
    public DefaultIdType Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? ImagePath { get; set; }
    public int SortOrder { get; set; }
    public DefaultIdType GalleryId { get; set; }
}