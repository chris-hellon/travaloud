using Travaloud.Application.Catalog.Enquiries.Dto;
using Travaloud.Application.Catalog.Enquiries.Queries;
using Travaloud.Domain.Catalog.Enquiries;

namespace Travaloud.Application.Catalog.Enquiries.Specification;

public class GeneralEnquiriesBySearchSpec : EntitiesByPaginationFilterSpec<GeneralEnquiry, GeneralEnquiryDto>
{
    public GeneralEnquiriesBySearchSpec(SearchGeneralEnquiriesRequest request)
        : base(request) =>
        Query
            .OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
            .Where(p => p.Name.Equals(request.Name), request.Name != null);
}