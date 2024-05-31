using Travaloud.Domain.Catalog.Properties;
using Travaloud.Shared.Common.Extensions;

namespace Travaloud.Domain.Catalog.Events;

public class Event : AuditableEntity, IAggregateRoot
{
    public Event(string name,
        string? description,
        string? shortDescription,
        string? imagePath,
        string? backgroundColor,
        DefaultIdType? propertyId)
    {
        Name = name;
        Description = description.FormatTextEditorString();
        ShortDescription = shortDescription.FormatTextEditorString();
        ImagePath = imagePath != null ? !imagePath.Contains("w-700") ? $"{imagePath}?w=700" : imagePath : null;
        BackgroundColor = backgroundColor;
        PropertyId = propertyId;
    }

    public string? Name { get; private set; }
    public string? Description { get; private set; }
    public string? ShortDescription { get; private set; }
    public string? ImagePath { get; private set; }
    public string? BackgroundColor { get; private set; }
    public DefaultIdType? PropertyId { get; private set; }

    public virtual Property Property { get; set; } = default!;

    public Event Update(string? name, string? description, string? shortDescription, string? imagePath, string? backgroundColor, DefaultIdType? propertyId)
    {
        if (name is not null && Name?.Equals(name) is not true) Name = name;
        if (description is not null && Description?.Equals(description) is not true) Description = description.FormatTextEditorString();
        if (backgroundColor is not null && BackgroundColor?.Equals(backgroundColor) is not true) BackgroundColor = backgroundColor;
        if (shortDescription is not null && ShortDescription?.Equals(shortDescription) is not true) ShortDescription = shortDescription.FormatTextEditorString();
        if (imagePath is not null && ImagePath?.Equals(imagePath) is not true) ImagePath = !imagePath.Contains("w-1000") ? $"{imagePath}?w=1000" : imagePath;
        if (propertyId is not null && PropertyId?.Equals(propertyId) is not true) PropertyId = propertyId;

        return this;
    }

    public Event ClearImagePath()
    {
        ImagePath = string.Empty;
        return this;
    }
}