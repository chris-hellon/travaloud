namespace Travaloud.Domain.Catalog.SEO;

public class Seo : AuditableEntity, IAggregateRoot
{
    public string Breadcrumbs { get; set; }
    public string SearchAction { get; set; }

    public Seo(string breadcrumbs = "", string searchAction = "")
    {
        Breadcrumbs = breadcrumbs;
        SearchAction = searchAction;
    }

    public Seo Update(string breadcrumbs = "", string? searchAction = "")
    {
        if (!string.IsNullOrEmpty(breadcrumbs) && Breadcrumbs?.Equals(breadcrumbs) is not true) Breadcrumbs = breadcrumbs;
        if (!string.IsNullOrEmpty(searchAction) && SearchAction?.Equals(searchAction) is not true) SearchAction = searchAction;

        return this;
    }
}