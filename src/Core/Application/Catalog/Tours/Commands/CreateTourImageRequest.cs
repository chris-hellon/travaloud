namespace Travaloud.Application.Catalog.Tours.Commands;

public class CreateTourImageRequest : IRequest<DefaultIdType>
{
    public DefaultIdType? TourId { get; set; }
    public string ImagePath { get; set; } = default!;
    public string ThumbnailImagePath { get; set; } = default!;
    public FileUploadRequest? Image { get; set; }
    public string? ImageInBytes { get; set; }
    public string? ImageExtension { get; set; }
    public int SortOrder { get; set; }
}