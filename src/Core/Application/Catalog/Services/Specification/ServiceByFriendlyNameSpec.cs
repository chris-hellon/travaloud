using Travaloud.Application.Catalog.Services.Dto;
using Travaloud.Domain.Catalog.Services;

namespace Travaloud.Application.Catalog.Services.Specification;

public class ServiceByFriendlyNameSpec : Specification<Service, ServiceDetailsDto>, ISingleResultSpecification<Service>
{
    public ServiceByFriendlyNameSpec(string name) =>
        Query.Where(p => p.Title.UrlFriendly() == name);
}