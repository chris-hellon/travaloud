using Travaloud.Domain.Catalog.Pages;

namespace Travaloud.Application.Catalog.PageModals.Specification;

public class PageModalByTitleSpec : Specification<PageModal>, ISingleResultSpecification<PageModal>
{
    public PageModalByTitleSpec(string title) =>
        Query.Where(p => p.Title == title);
}