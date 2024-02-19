namespace Travaloud.Domain.Catalog.Properties;

public class PropertyFacility : AuditableEntity, IAggregateRoot
{
    public PropertyFacility(string title)
    {
        Title = title;
    }

    public DefaultIdType PropertyId { get; private set; }
    public string Title { get; private set; }

    public PropertyFacility Update(string title)
    {
        if (title is not null && Title?.Equals(title) is not true) Title = title;
        return this;
    }
}