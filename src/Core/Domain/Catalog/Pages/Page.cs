namespace Travaloud.Domain.Catalog.Pages;

public class Page : AuditableEntity, IAggregateRoot
{
    public Page(string title,
        string? metaKeywords,
        string? metaDescription,
        string? metaImageUrl,
        string? customSeoScripts,
        string? urlSlug,
        string? h1,
        string? h2,
        string? h3,
        string? seoPageTitle)
    {
        Title = title;
        MetaKeywords = metaKeywords;
        MetaDescription = metaDescription;
        MetaImageUrl = metaImageUrl;
        CustomSeoScripts = customSeoScripts;
        UrlSlug = urlSlug;
        H1 = h1;
        H2 = h2;
        H3 = h3;
        SeoPageTitle = seoPageTitle;
    }

    public string Title { get; set; }
    public string? MetaKeywords { get; private set; }
    public string? MetaDescription { get; private set; }
    public string? MetaImageUrl { get; private set; }
    public string? CustomSeoScripts { get; private set; }
    public string? UrlSlug { get; private set; }
    public string? H1 { get; private set; }
    public string? H2 { get; private set; }
    public string? H3 { get; private set; }
    public string? SeoPageTitle { get; private set; }
    
    public virtual IList<PageSorting>? PageSortings { get; private set; }
    public virtual IList<PageModalLookup>? PageModalLookups { get; private set; }

    public Page Update(string? title,
        string? metaKeywords,
        string? metaDescription,
        string? metaImageUrl,
        string? customSeoScripts,
        string? urlSlug,
        string? h1,
        string? h2,
        string? h3,
        string? seoPageTitle)
    {
        if (title is not null && Title?.Equals(title) is not true) Title = title;
        if (metaImageUrl is not null && MetaImageUrl?.Equals(metaImageUrl) is not true) MetaImageUrl = metaImageUrl;
        if (metaKeywords is not null && MetaKeywords?.Equals(metaKeywords) is not true) MetaKeywords = metaKeywords;
        if (metaDescription is not null && MetaDescription?.Equals(metaDescription) is not true) MetaDescription = metaDescription;
        if (customSeoScripts is not null && CustomSeoScripts?.Equals(customSeoScripts) is not true) CustomSeoScripts = customSeoScripts;
        if (urlSlug is not null && UrlSlug?.Equals(urlSlug) is not true) UrlSlug = urlSlug;
        if (h1 is not null && H1?.Equals(h1) is not true) H1 = h1;
        if (h2 is not null && H2?.Equals(h2) is not true) H2 = h2;
        if (h3 is not null && H3?.Equals(h3) is not true) H3 = h3;
        if (seoPageTitle is not null && SeoPageTitle?.Equals(seoPageTitle) is not true) SeoPageTitle = seoPageTitle;
        
        return this;
    }
}