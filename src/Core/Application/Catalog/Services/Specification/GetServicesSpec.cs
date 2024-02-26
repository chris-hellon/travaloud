using Travaloud.Application.Catalog.Services.Dto;
using Travaloud.Application.Catalog.Services.Queries;
using Travaloud.Domain.Catalog.Services;

namespace Travaloud.Application.Catalog.Services.Specification;

public class GetServicesSpec : Specification<Service, ServiceDto>
{
    public GetServicesSpec(GetServicesRequest request) =>
        Query
            .OrderBy(c => c.Title);
}