using Travaloud.Application.Catalog.Galleries.Dto;
using Travaloud.Application.Catalog.Galleries.Queries;
using Travaloud.Domain.Catalog.Galleries;

namespace Travaloud.Application.Catalog.Galleries.Specification;

public class GalleriesBySearchSpec : EntitiesByPaginationFilterSpec<Gallery, GalleryDto>
{
    public GalleriesBySearchSpec(SearchGalleriesRequest request)
        : base(request) =>
        Query
            .OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
            .Where(p => p.Title.Equals(request.Title), request.Title != null)
            .Where(p => p.CreatedOn.Equals(request.CreatedOn), request.CreatedOn != null)
            .Where(p => p.CreatedBy.Equals(request.CreatedBy), request.CreatedBy != null);
}