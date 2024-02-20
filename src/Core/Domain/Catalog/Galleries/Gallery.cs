namespace Travaloud.Domain.Catalog.Galleries;

public class Gallery : AuditableEntity, IAggregateRoot
{
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public string? MetaKeywords { get; private set; }
    public string? MetaDescription { get; private set; }

    public virtual IList<GalleryImage> GalleryImages { get; set; } = default!;

    public Gallery(string title, string? description, string? metaKeywords, string? metaDescription)
    {
        Title = title;
        Description = description;
        MetaKeywords = metaKeywords;
        MetaDescription = metaDescription;
    }
    
    public Gallery Update(string? title, string? description, string? metaKeywords, string? metaDescription)
    {
        if (title is not null && Title != title)
            Title = title;

        if (description is not null && Description != description)
            Description = description;

        if (metaKeywords is not null && MetaKeywords != metaKeywords)
            MetaKeywords = metaKeywords;
        
        if (metaDescription is not null && MetaDescription != metaDescription)
            MetaDescription = metaDescription;
        
        return this;
    }

}