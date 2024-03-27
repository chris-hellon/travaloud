namespace Travaloud.Application.Catalog.Tours.Commands;

public class TourItinerarySectionImageRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType? TourItinerarySectionId { get; set; }
    public string ImagePath { get; set; } = default!;
    public string ThumbnailImagePath { get; set; } = default!;
    public bool IsCreate { get; set; }
    public FileUploadRequest? Image { get; set; }
    public bool DeleteCurrentImage { get; set; }
    public string? ImageInBytes { get; set; }
    public string? ImageExtension { get; set; }
}