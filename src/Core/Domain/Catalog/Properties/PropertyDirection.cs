namespace Travaloud.Domain.Catalog.Properties;

public class PropertyDirection : AuditableEntity, IAggregateRoot
{
    public PropertyDirection(string title)
    {
        Title = title;
    }

    public DefaultIdType PropertyId { get; private set; }
    public string Title { get; private set; }

    public virtual IList<PropertyDirectionContent> Content { get; set; } = default!;

    public PropertyDirection Update(string title)
    {
        if (title is not null && Title?.Equals(title) is not true) Title = title;
        return this;
    }
}