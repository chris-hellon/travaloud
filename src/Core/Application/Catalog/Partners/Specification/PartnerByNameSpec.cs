using Travaloud.Domain.Catalog.Partners;

namespace Travaloud.Application.Catalog.Partners.Specification;

public class PartnerByNameSpec : Specification<Partner>, ISingleResultSpecification<Partner>
{
    public PartnerByNameSpec(string name) =>
        Query.Where(p => p.Name == name);
}