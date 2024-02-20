using Travaloud.Application.Catalog.Galleries.Dto;
using Travaloud.Domain.Catalog.Galleries;

namespace Travaloud.Application.Catalog.Galleries.Specification;

public class GalleryByIdSpec : Specification<Gallery, GalleryDetailsDto>, ISingleResultSpecification<Gallery>
{
    public GalleryByIdSpec(DefaultIdType id) =>
        Query
            .Where(p => p.Id == id)
            .Include(p => p.GalleryImages);
}