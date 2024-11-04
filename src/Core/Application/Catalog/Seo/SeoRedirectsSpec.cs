using Travaloud.Domain.Catalog.SEO;

namespace Travaloud.Application.Catalog.Seo;

public class SeoRedirectsSpec : EntitiesByPaginationFilterSpec<SeoRedirect, SeoRedirectDto>
{
    public SeoRedirectsSpec(GetSeoRedirectsRequest request)
        : base(request) =>
        Query.OrderByDescending(p => p.CreatedOn);
}