using Travaloud.Application.Catalog.Pages.Dto;
using Travaloud.Application.Catalog.Pages.Queries;
using Travaloud.Domain.Catalog.Pages;

namespace Travaloud.Application.Catalog.Pages.Specification;

public class PagesBySearchSpec : EntitiesByPaginationFilterSpec<Page, PageDto>
{
    public PagesBySearchSpec(SearchPagesRequest request)
        : base(request) =>
        Query
            .OrderBy(c => c.Title, !request.HasOrderBy())
            .Where(p => p.Title.Equals(request.Title), request.Title != null);
}