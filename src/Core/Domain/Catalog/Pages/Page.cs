namespace Travaloud.Domain.Catalog.Pages;

public class Page : AuditableEntity, IAggregateRoot
{
    public Page(string title, string? metaKeywords, string? metaDescription, string? metaImageUrl)
    {
        Title = title;
        MetaKeywords = metaKeywords;
        MetaDescription = metaDescription;
        MetaImageUrl = metaImageUrl;
    }

    public string Title { get; set; }
    public string? MetaKeywords { get; private set; }
    public string? MetaDescription { get; private set; }
    public string? MetaImageUrl { get; private set; }

    public virtual IList<PageSorting>? PageSortings { get; private set; }
    public virtual IList<PageModalLookup>? PageModalLookups { get; private set; }

    public Page Update(string? title, string? metaKeywords, string? metaDescription, string? metaImageUrl)
    {
        if (title is not null && Title?.Equals(title) is not true) Title = title;
        if (metaImageUrl is not null && MetaImageUrl?.Equals(metaImageUrl) is not true) MetaImageUrl = metaImageUrl;
        if (metaKeywords is not null && MetaKeywords?.Equals(metaKeywords) is not true) MetaKeywords = metaKeywords;
        if (metaDescription is not null && MetaDescription?.Equals(metaDescription) is not true) MetaDescription = metaDescription;

        return this;
    }
}