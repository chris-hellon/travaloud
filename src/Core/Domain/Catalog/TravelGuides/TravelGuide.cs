namespace Travaloud.Domain.Catalog.TravelGuides;

public class TravelGuide : AuditableEntity, IAggregateRoot
{
    public string Title { get; private set; } = default!;
    public string SubTitle { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string ShortDescription { get; private set; } = default!;
    public string? ImagePath { get; private set; }
    public string? MetaKeywords { get; private set; }
    public string? MetaDescription { get; private set; }

    public virtual IEnumerable<TravelGuideGalleryImage>? TravelGuideGalleryImages { get; set; }

    public TravelGuide(string title, string subTitle, string description, string shortDescription, string? imagePath, string? metaKeywords, string? metaDescription)
    {
        Title = title;
        SubTitle = subTitle;
        Description = description;
        ShortDescription = shortDescription;
        ImagePath = imagePath;
        MetaKeywords = metaKeywords;
        MetaDescription = metaDescription;
    }
    
    public TravelGuide Update(string? title, string? subTitle, string? description, string? shortDescription, string? imagePath, string? metaKeywords, string? metaDescription)
    {
        if (title is not null && Title != title)
            Title = title;

        if (subTitle is not null && SubTitle != subTitle)
            SubTitle = subTitle;

        if (description is not null && Description != description)
            Description = description;

        if (shortDescription is not null && ShortDescription != shortDescription)
            ShortDescription = shortDescription;

        if (imagePath is not null && ImagePath != imagePath)
            ImagePath = imagePath;
        
        if (metaKeywords is not null && MetaKeywords != metaKeywords)
            MetaKeywords = metaKeywords;
        
        if (metaDescription is not null && MetaDescription != metaDescription)
            MetaDescription = metaDescription;

        return this;
    }

    
    public TravelGuide ClearImagePath()
    {
        ImagePath = string.Empty;
        return this;
    }
}