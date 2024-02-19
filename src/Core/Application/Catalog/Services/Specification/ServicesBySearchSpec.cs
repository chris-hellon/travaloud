using Travaloud.Application.Catalog.Services.Dto;
using Travaloud.Application.Catalog.Services.Queries;
using Travaloud.Domain.Catalog.Services;

namespace Travaloud.Application.Catalog.Services.Specification;

public class ServicesBySearchSpec : EntitiesByPaginationFilterSpec<Service, ServiceDto>
{
    public ServicesBySearchSpec(SearchServiceRequest request)
        : base(request) =>
        Query
            .OrderBy(c => c.Title, !request.HasOrderBy())
            .Where(p => p.Title.Equals(request.Name), request.Name != null);
}