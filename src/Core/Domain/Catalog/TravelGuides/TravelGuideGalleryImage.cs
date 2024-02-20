namespace Travaloud.Domain.Catalog.TravelGuides;

public class TravelGuideGalleryImage : AuditableEntity, IAggregateRoot
{
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public string? ImagePath { get; private set; }
    public int SortOrder { get; private set; }
    public DefaultIdType TravelGuideId { get; private set; }

    public TravelGuideGalleryImage(string title, string? description, string? imagePath, int sortOrder)
    {
        Title = title;
        Description = description;
        ImagePath = imagePath;
        SortOrder = sortOrder;
    }
    
    public TravelGuideGalleryImage(string title, string? description, string? imagePath, int sortOrder, DefaultIdType travelGuideId)
    {
        Title = title;
        Description = description;
        ImagePath = imagePath;
        SortOrder = sortOrder;
        TravelGuideId = travelGuideId;
    }
    
    public TravelGuideGalleryImage Update(string? title, string? description, string? imagePath, int? sortOrder)
    {
        if (title is not null && Title != title)
            Title = title;

        if (description is not null && Description != description)
            Description = description;

        if (imagePath is not null && ImagePath != imagePath)
            ImagePath = imagePath;

        if (sortOrder.HasValue && SortOrder != sortOrder)
            SortOrder = sortOrder.Value;

        return this;
    }
    
    public TravelGuideGalleryImage Update(string? title, string? description, string? imagePath, int? sortOrder, DefaultIdType travelGuideId)
    {
        if (title is not null && Title != title)
            Title = title;

        if (description is not null && Description != description)
            Description = description;

        if (imagePath is not null && ImagePath != imagePath)
            ImagePath = imagePath;

        if (sortOrder.HasValue && SortOrder != sortOrder)
            SortOrder = sortOrder.Value;

        TravelGuideId = travelGuideId;
        
        return this;
    }

    
    public TravelGuideGalleryImage ClearImagePath()
    {
        ImagePath = string.Empty;
        return this;
    }
}