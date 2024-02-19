namespace Travaloud.Application.Catalog.PageModals.Commands;

public class PageModalPageRequest
{
    public DefaultIdType Id { get; set; } = default!;
    
    public DefaultIdType PageId { get; set; } = default!;
    
    public string PageName { get; set; } = default!;
    
    public override int GetHashCode() => PageName?.GetHashCode() ?? 0;
    
    public override string ToString() => PageName;
    
    public override bool Equals(object? o)
    {
        var other = o as PageModalPageRequest;
        return other?.PageName == PageName;
    }
}