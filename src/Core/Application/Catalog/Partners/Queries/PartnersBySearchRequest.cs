using Travaloud.Application.Catalog.Partners.Dto;
using Travaloud.Domain.Catalog.Partners;

namespace Travaloud.Application.Catalog.Partners.Queries;

public class PartnersBySearchRequest : EntitiesByPaginationFilterSpec<Partner, PartnerDto>
{
    public PartnersBySearchRequest(SearchPartnersRequest request)
        : base(request) =>
        Query
            .OrderBy(c => c.Name, !request.HasOrderBy())
            .Where(p => p.Name.Equals(request.Name), request.Name != null);
}