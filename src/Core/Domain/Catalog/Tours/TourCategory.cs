namespace Travaloud.Domain.Catalog.Tours;

public class TourCategory : AuditableEntity, IAggregateRoot
{
    public TourCategory(string name,
        string? description,
        string? iconClass,
        string? shortDescription,
        string? imagePath,
        string? thumbnailImagePath,
        string? metaKeywords,
        string? metaDescription,
        bool? topLevelCategory,
        DefaultIdType? parentTourCategoryId)
    {
        Name = name;
        Description = description;
        IconClass = iconClass;
        ShortDescription = shortDescription;
        ImagePath = imagePath != null ? !imagePath.Contains("w-1000") ? $"{imagePath}?w=1000" : imagePath : null;
        ThumbnailImagePath = thumbnailImagePath != null ? !thumbnailImagePath.Contains("w-700") ? $"{thumbnailImagePath}?w=700" : thumbnailImagePath : null;
        MetaKeywords = metaKeywords;
        MetaDescription = metaDescription;
        TopLevelCategory = topLevelCategory;
        ParentTourCategoryId = parentTourCategoryId;
    }

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? IconClass { get; private set; }
    public string? ShortDescription { get; private set; }
    public string? ImagePath { get; private set; }
    public string? ThumbnailImagePath { get; private set; }
    public string? MetaKeywords { get; private set; }
    public string? MetaDescription { get; private set; }
    public bool? TopLevelCategory { get; private set; }
    public DefaultIdType? ParentTourCategoryId { get; private set; }

    public TourCategory Update(string name, string? description, string? iconClass, string? shortDescription, string? imagePath, string? thumbnailImagePath, string? metaKeywords, string? metaDescription, bool? topLevelCategory, DefaultIdType? parentTourCategoryId)
    {
        if (name is not null && Name?.Equals(name) is not true) Name = name;
        if (description is not null && Description?.Equals(description) is not true) Description = description;
        if (iconClass is not null && IconClass?.Equals(iconClass) is not true) IconClass = iconClass;
        if (shortDescription is not null && ShortDescription?.Equals(shortDescription) is not true) ShortDescription = shortDescription;
        if (imagePath is not null && ImagePath?.Equals(imagePath) is not true) ImagePath = !imagePath.Contains("w-1000") ? $"{imagePath}?w=1000" : imagePath;
        if (thumbnailImagePath is not null && ThumbnailImagePath?.Equals(thumbnailImagePath) is not true) ThumbnailImagePath = thumbnailImagePath.Contains("w-700") ? $"{thumbnailImagePath}?w=700" : thumbnailImagePath;
        if (metaKeywords is not null && MetaKeywords?.Equals(metaKeywords) is not true) MetaKeywords = metaKeywords;
        if (metaDescription is not null && MetaDescription?.Equals(metaDescription) is not true) MetaDescription = metaDescription;
        if (topLevelCategory.HasValue && TopLevelCategory != topLevelCategory) TopLevelCategory = topLevelCategory.Value;
        if (parentTourCategoryId.HasValue && ParentTourCategoryId != parentTourCategoryId) ParentTourCategoryId = parentTourCategoryId.Value;

        return this;
    }

    public TourCategory ClearImagePath()
    {
        ImagePath = string.Empty;
        ThumbnailImagePath = string.Empty;
        return this;
    }
}