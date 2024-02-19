using Travaloud.Application.Catalog.TourEnquiries.Dto;
using Travaloud.Application.Catalog.TourEnquiries.Queries;
using Travaloud.Domain.Catalog.Enquiries;

namespace Travaloud.Application.Catalog.TourEnquiries.Specification;

public class TourEnquiriesBySearchSpec : EntitiesByPaginationFilterSpec<TourEnquiry, TourEnquiryDto>
{
    public TourEnquiriesBySearchSpec(SearchTourEnquiriesRequest request)
        : base(request) =>
        Query
            .OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
            .Where(p => p.Name.Equals(request.Name), request.Name != null);
}