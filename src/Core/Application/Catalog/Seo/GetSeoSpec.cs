namespace Travaloud.Application.Catalog.Seo;


public class GetSeoSpec : Specification<Domain.Catalog.SEO.Seo, SeoDetailsDto>, ISingleResultSpecification<Domain.Catalog.SEO.Seo>
{
    public GetSeoSpec() => Query.Take(1);
}