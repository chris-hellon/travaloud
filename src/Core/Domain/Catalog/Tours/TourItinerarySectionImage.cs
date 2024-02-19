namespace Travaloud.Domain.Catalog.Tours;

public class TourItinerarySectionImage : AuditableEntity, IAggregateRoot
{
    public DefaultIdType TourItinerarySectionId { get; private set; }
    public string ImagePath { get; private set; } = default!;
    public string ThumbnailImagePath { get; private set; } = default!;

    public TourItinerarySectionImage()
    {

    }

    public TourItinerarySectionImage(string imagePath, string thumbnailImagePath, DefaultIdType? tourItinerarySectionId = null)
    {
        if (tourItinerarySectionId.HasValue)
        {
            TourItinerarySectionId = tourItinerarySectionId.Value;
        }

        ImagePath = !imagePath.Contains("w-1000") ? $"{imagePath}?w=1000" : imagePath;
        ThumbnailImagePath = !thumbnailImagePath.Contains("w-700") ? $"{thumbnailImagePath}?w=700" : thumbnailImagePath;
    }

    public TourItinerarySectionImage Update(DefaultIdType? tourItinerarySectionId, string? imagePath, string? thumbnailImagePath)
    {
        if (tourItinerarySectionId.HasValue && tourItinerarySectionId.Value != DefaultIdType.Empty && !TourItinerarySectionId.Equals(tourItinerarySectionId.Value)) TourItinerarySectionId = tourItinerarySectionId.Value;
        if (imagePath is not null && ImagePath?.Equals(imagePath) is not true) ImagePath = !imagePath.Contains("w-1000") ? $"{imagePath}?w=1000" : imagePath;
        if (thumbnailImagePath is not null && ThumbnailImagePath?.Equals(thumbnailImagePath) is not true) ThumbnailImagePath = thumbnailImagePath.Contains("w-700") ? $"{thumbnailImagePath}?w=700" : thumbnailImagePath;
        return this;
    }

    public TourItinerarySectionImage ClearImagePath()
    {
        ImagePath = string.Empty;
        ThumbnailImagePath = string.Empty;
        return this;
    }
}