namespace Travaloud.Domain.Catalog.Pages;

public class PageModalLookup : AuditableEntity, IAggregateRoot
{
    public PageModalLookup(DefaultIdType id, DefaultIdType pageId, DefaultIdType pageModalId)
    {
        Id = id;
        PageId = pageId;
        PageModalId = pageModalId;
    }

    public DefaultIdType PageId { get; set; }
    public DefaultIdType PageModalId { get; set; }

    public virtual Page Page { get; set; } = default!;
    public virtual PageModal PageModal { get; set; } = default!;
}