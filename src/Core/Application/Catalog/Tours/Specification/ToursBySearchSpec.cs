using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class ToursBySearchSpec : EntitiesByPaginationFilterSpec<Tour, TourDto>
{
    public ToursBySearchSpec(SearchToursRequest request)
        : base(request) =>
        Query
            .OrderBy(c => c.Name, !request.HasOrderBy())
            .Where(p => p.Name.Equals(request.Name), request.Name != null);
}

