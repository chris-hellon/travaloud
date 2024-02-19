using Travaloud.Domain.Catalog.Services;

namespace Travaloud.Application.Catalog.Services.Specification;

public class ServiceByNameSpec : Specification<Service>, ISingleResultSpecification<Service>
{
    public ServiceByNameSpec(string name) =>
        Query.Where(p => p.Title == name);
}