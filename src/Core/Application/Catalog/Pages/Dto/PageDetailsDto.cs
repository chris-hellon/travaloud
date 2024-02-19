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
    public IList<PageModalDto>? PageModals { get; set; }
    public IList<PageSortingDto>? PageSortings { get; set; }
}