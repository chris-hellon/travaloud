using Travaloud.Application.Catalog.PageModals.Dto;
using Travaloud.Application.Catalog.PageSorting.Dto;

namespace Travaloud.Application.Catalog.Pages.Dto;

public class PageDetailsDto
{
    public DefaultIdType Id { get; set; }
    public string Title { get; set; } = default!;
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaImageUrl { get; set; }
    public string? CustomSeoScripts { get; set; }
    public string? UrlSlug { get; set; }
    public string? H1 { get; set; }
    public string? H2 { get; set; }
    public string? H3 { get; set; }
    public string? SeoPageTitle { get; set; }
    public IList<PageModalDto>? PageModals { get; set; }
    public IList<PageSortingDto>? PageSortings { get; set; }
}