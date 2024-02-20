using Travaloud.Domain.Catalog.Galleries;

namespace Travaloud.Application.Catalog.Galleries.Specification;

public class GalleryByTitleSpec : Specification<Gallery>, ISingleResultSpecification<Gallery>
{
    public GalleryByTitleSpec(string name) =>
        Query.Where(p => p.Title == name);
}