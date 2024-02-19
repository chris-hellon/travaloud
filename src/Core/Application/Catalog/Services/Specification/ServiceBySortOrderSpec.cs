using Travaloud.Domain.Catalog.Services;

namespace Travaloud.Application.Catalog.Services.Specification;

public class ServiceBySortOrderSpec : Specification<Service>, ISingleResultSpecification<Service>
{
    public ServiceBySortOrderSpec() =>
        Query.OrderByDescending(x => x.SortOrder);
}