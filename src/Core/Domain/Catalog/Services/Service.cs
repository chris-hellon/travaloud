namespace Travaloud.Domain.Catalog.Services;

public class Service : AuditableEntity, IAggregateRoot
{
    public string Title { get; private set; } = default!;
    public string? SubTitle { get; private set; }
    public string Description { get; private set; } = default!;
    public string? ShortDescription { get; private set; }
    public string? BodyHtml { get; private set; }
    public string? IconClass { get; set; }
    public int? SortOrder { get; set; }

    public virtual IList<ServiceField>? ServiceFields { get; set; }

    public Service()
    {
    }

    public Service(string title, string? subTitle, string description, string? shortDescription, string? bodyHtml, string? iconClass)
    {
        Title = title;
        SubTitle = subTitle;
        Description = description;
        ShortDescription = shortDescription;
        BodyHtml = bodyHtml;
        IconClass = iconClass;
    }

    public Service Update(string? title, string? subTitle, string? description, string? shortDescription, string? bodyHtml, string? iconClass)
    {
        if (title is not null && Title?.Equals(title) is not true) Title = title;
        if (subTitle is not null && SubTitle?.Equals(subTitle) is not true) SubTitle = subTitle;
        if (description is not null && Description?.Equals(description) is not true) Description = description;
        if (shortDescription is not null && ShortDescription?.Equals(shortDescription) is not true) ShortDescription = shortDescription;
        if (bodyHtml is not null && BodyHtml?.Equals(bodyHtml) is not true) BodyHtml = bodyHtml;
        if (iconClass is not null && IconClass?.Equals(iconClass) is not true) IconClass = iconClass;
        return this;
    }
}