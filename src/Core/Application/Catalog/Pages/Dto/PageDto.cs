namespace Travaloud.Application.Catalog.Pages.Dto;

public class PageDto 
{
    public DefaultIdType Id { get; set; }
    public string Title { get; set; } = default!;
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaImageUrl { get; set; }
}