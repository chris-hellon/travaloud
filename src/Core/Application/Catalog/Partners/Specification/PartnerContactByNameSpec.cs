using Travaloud.Domain.Catalog.Partners;

namespace Travaloud.Application.Catalog.Partners.Specification;

public class PartnerContactByNameSpec : Specification<PartnerContact>, ISingleResultSpecification<PartnerContact>
{
    public PartnerContactByNameSpec(string name) =>
        Query.Where(p => p.Name == name);
}