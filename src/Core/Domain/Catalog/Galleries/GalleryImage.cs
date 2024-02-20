namespace Travaloud.Domain.Catalog.Galleries;

public class GalleryImage : AuditableEntity, IAggregateRoot
{
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public string? ImagePath { get; private set; }
    public int SortOrder { get; private set; }
}