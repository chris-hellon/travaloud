namespace Travaloud.Domain.Catalog.Tours;

public class TourImage : AuditableEntity, IAggregateRoot
{
    public DefaultIdType TourId { get; private set; }
    public string ImagePath { get; private set; } = default!;
    public string ThumbnailImagePath { get; private set; } = default!;
    public int? SortOrder { get; set; }
    
    public TourImage()
    {
    }

    public TourImage(string imagePath, string thumbnailImagePath, int sortOrder, DefaultIdType? tourId = null)
    {
        if (tourId.HasValue)
        {
            TourId = tourId.Value;
        }

        SortOrder = sortOrder;
        ImagePath = !imagePath.Contains("w-1000") ? $"{imagePath}?w=1000" : imagePath;
        ThumbnailImagePath = !thumbnailImagePath.Contains("w-700") ? $"{thumbnailImagePath}?w=700" : thumbnailImagePath;
    }

    public TourImage Update(string? imagePath, string? thumbnailImagePath, int sortOrder, DefaultIdType? tourId = null)
    {
        if (tourId.HasValue && tourId.Value != DefaultIdType.Empty && !TourId.Equals(tourId.Value)) TourId = tourId.Value;
        if (imagePath is not null && ImagePath?.Equals(imagePath) is not true) ImagePath = !imagePath.Contains("w-1000") ? $"{imagePath}?w=1000" : imagePath;
        if (thumbnailImagePath is not null && ThumbnailImagePath?.Equals(thumbnailImagePath) is not true) ThumbnailImagePath = thumbnailImagePath.Contains("w-700") ? $"{thumbnailImagePath}?w=700" : thumbnailImagePath;
        SortOrder = sortOrder;
        return this;
    }

    public TourImage ClearImagePath()
    {
        ImagePath = string.Empty;
        ThumbnailImagePath = string.Empty;
        return this;
    }
}