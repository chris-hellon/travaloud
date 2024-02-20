namespace Travaloud.Application.Catalog.TravelGuides.Commands;

public class TravelGuideGalleryImageRequest : IRequest<DefaultIdType>
{
    public DefaultIdType? Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? ImagePath { get; set; }
    public FileUploadRequest? Image { get; set; }
    public string? ImageInBytes { get; set; }
    public string? ImageExtension { get; set; }
    public bool DeleteCurrentImage { get; set; }
    public int SortOrder { get; set; }
}