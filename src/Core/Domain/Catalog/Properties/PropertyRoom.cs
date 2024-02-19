using Travaloud.Domain.Catalog.Images;

namespace Travaloud.Domain.Catalog.Properties;

public class PropertyRoom : AuditableEntity, IAggregateRoot
{
    public DefaultIdType PropertyId { get; private set; }
    public string Name { get; private set; } = default!;
    public string Description { get; private set; }
    public string? ShortDescription { get; private set; }
    public string? ImagePath { get; private set; }
    public string? PageTitle { get; private set; }
    public string? PageSubTitle { get; private set; }

    public virtual IList<Image>? Images { get; set; }

    public PropertyRoom(string name, string description, string? shortDescription, string? imagePath)
    {
        Name = name;
        Description = description;
        ShortDescription = shortDescription;
        ImagePath = imagePath != null ? !imagePath.Contains("w-1000") ? $"{imagePath}?w=1000" : imagePath : null;
    }

    public PropertyRoom Update(string? name, string? description, string? shortDescription, string? imagePath)
    {
        if (name is not null && Name?.Equals(name) is not true) Name = name;
        if (description is not null && Description?.Equals(description) is not true) Description = description;
        if (shortDescription is not null && ShortDescription?.Equals(shortDescription) is not true) ShortDescription = shortDescription;
        if (imagePath is not null && ImagePath?.Equals(imagePath) is not true) ImagePath = !imagePath.Contains("w-1000") ? $"{imagePath}?w=1000" : imagePath;
        return this;
    }

    public PropertyRoom ClearImagePath()
    {
        ImagePath = string.Empty;
        return this;
    }
}