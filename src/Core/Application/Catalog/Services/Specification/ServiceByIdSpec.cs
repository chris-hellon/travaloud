using Travaloud.Application.Catalog.Services.Dto;
using Travaloud.Domain.Catalog.Services;

namespace Travaloud.Application.Catalog.Services.Specification;

public class ServiceByIdSpec : Specification<Service, ServiceDetailsDto>, ISingleResultSpecification<Service>
{
    public ServiceByIdSpec(DefaultIdType id) =>
        Query
            .Where(p => p.Id == id)
            .Include(p => p.ServiceFields);
}