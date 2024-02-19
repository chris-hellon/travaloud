namespace Travaloud.Domain.Catalog.Properties;

public class PropertyImage : AuditableEntity, IAggregateRoot
{
    public DefaultIdType PropertyId { get; private set; }
    public string ImagePath { get; private set; } = default!;
    public string ThumbnailImagePath { get; private set; } = default!;
    public int? SortOrder { get; set; }
        
    public PropertyImage()
    {
    }

    public PropertyImage(string imagePath, string thumbnailImagePath, int sortOrder, DefaultIdType? propertyId = null)
    {
        if (propertyId.HasValue)
        {
            PropertyId = propertyId.Value;
        }

        SortOrder = sortOrder;
        ImagePath = !imagePath.Contains("w-1000") ? $"{imagePath}?w=1000" : imagePath;
        ThumbnailImagePath = !thumbnailImagePath.Contains("w-700") ? $"{thumbnailImagePath}?w=700" : thumbnailImagePath;
    }

    public PropertyImage Update(string? imagePath, string? thumbnailImagePath, int sortOrder, DefaultIdType? propertyId = null)
    {
        if (propertyId.HasValue && propertyId.Value != DefaultIdType.Empty && !PropertyId.Equals(propertyId.Value)) PropertyId = propertyId.Value;
        if (imagePath is not null && ImagePath?.Equals(imagePath) is not true) ImagePath = !imagePath.Contains("w-1000") ? $"{imagePath}?w=1000" : imagePath;
        if (thumbnailImagePath is not null && ThumbnailImagePath?.Equals(thumbnailImagePath) is not true) ThumbnailImagePath = thumbnailImagePath.Contains("w-700") ? $"{thumbnailImagePath}?w=700" : thumbnailImagePath;
        SortOrder = sortOrder;
        return this;
    }

    public PropertyImage ClearImagePath()
    {
        ImagePath = string.Empty;
        ThumbnailImagePath = string.Empty;
        return this;
    }
}