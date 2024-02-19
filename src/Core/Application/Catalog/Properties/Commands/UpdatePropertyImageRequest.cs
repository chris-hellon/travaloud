namespace Travaloud.Application.Catalog.Properties.Commands;

public class UpdatePropertyImageRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType PropertyId { get; set; }
    public string ImagePath { get; set; } = default!;
    public string ThumbnailImagePath { get; set; } = default!;
    public FileUploadRequest? Image { get; set; }
    public bool DeleteCurrentImage { get; set; }
    public string? ImageInBytes { get; set; }
    public string? ImageExtension { get; set; }
    public int SortOrder { get; set; }
    public string? SlideshowSelector { get; set; } = "1";
}