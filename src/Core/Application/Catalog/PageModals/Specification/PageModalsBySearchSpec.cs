using Travaloud.Application.Catalog.PageModals.Dto;
using Travaloud.Application.Catalog.PageModals.Queries;
using Travaloud.Domain.Catalog.Pages;

namespace Travaloud.Application.Catalog.PageModals.Specification;

public class PageModalsBySearchSpec : EntitiesByPaginationFilterSpec<PageModal, PageModalDto>
{
    public PageModalsBySearchSpec(SearchPageModalsRequest request)
        : base(request) =>
        Query
            .OrderBy(c => c.Title, !request.HasOrderBy())
            .Where(p => p.Title.Equals(request.Title), request.Title != null);
}