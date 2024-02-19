using Travaloud.Domain.Catalog.Pages;

namespace Travaloud.Application.Catalog.PageModals.Specification;

public class PageModalByIdSpec : Specification<PageModal>, ISingleResultSpecification<PageModal>
{
    public PageModalByIdSpec(DefaultIdType id)
    {
        Query
            .Include(x => x.PageModalLookups)!
            .ThenInclude(x => x.Page)
            .Where(p => p.Id == id);
    }
}