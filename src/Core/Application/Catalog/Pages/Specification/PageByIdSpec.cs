using Travaloud.Domain.Catalog.Pages;

namespace Travaloud.Application.Catalog.Pages.Specification;

public class PageByIdSpec : Specification<Page>, ISingleResultSpecification<Page>
{
    public PageByIdSpec(DefaultIdType id)
    {
        Query
            .Include(x => x.PageSortings)
            .Include(x => x.PageModalLookups)!
            .ThenInclude(x => x.PageModal)
            .Where(p => p.Id == id).AsSplitQuery();
    }
}