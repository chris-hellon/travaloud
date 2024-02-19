namespace Travaloud.Application.Catalog.PageModals.Dto;

public class PageModalLookupDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType PageId { get; set; }
    public DefaultIdType PageModalId { get; set; }
    public string Title { get; set; } = default!;
}