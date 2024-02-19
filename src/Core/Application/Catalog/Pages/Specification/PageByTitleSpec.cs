using Travaloud.Domain.Catalog.Pages;

namespace Travaloud.Application.Catalog.Pages.Specification;

public class PageByTitleSpec : Specification<Page>, ISingleResultSpecification<Page>
{
    public PageByTitleSpec(string title) =>
        Query.Where(p => p.Title == title);
}