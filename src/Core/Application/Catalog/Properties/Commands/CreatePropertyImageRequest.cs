namespace Travaloud.Application.Catalog.Properties.Commands;

public class CreatePropertyImageRequest : IRequest<DefaultIdType>
{
    public DefaultIdType? PropertyId { get; set; }
    public string ImagePath { get; set; } = default!;
    public string ThumbnailImagePath { get; set; } = default!;
    public FileUploadRequest? Image { get; set; }
    public string? ImageInBytes { get; set; }
    public string? ImageExtension { get; set; }
    public int SortOrder { get; set; }
}