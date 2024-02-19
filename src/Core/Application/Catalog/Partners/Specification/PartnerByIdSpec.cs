using Travaloud.Application.Catalog.Partners.Dto;
using Travaloud.Domain.Catalog.Partners;

namespace Travaloud.Application.Catalog.Partners.Specification;

public class PartnerByIdSpec : Specification<Partner, PartnerDetailsDto>, ISingleResultSpecification<Partner>
{
    public PartnerByIdSpec(DefaultIdType id) =>
        Query
            .Where(p => p.Id == id)
            .Include(p => p.PartnerContacts);
}