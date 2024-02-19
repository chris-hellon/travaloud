using Travaloud.Application.Catalog.TourEnquiries.Dto;
using Travaloud.Domain.Catalog.Enquiries;

namespace Travaloud.Application.Catalog.TourEnquiries.Specification;

public class TourEnquriyByIdSpec : Specification<TourEnquiry, TourEnquiryDto>, ISingleResultSpecification<TourEnquiry>
{
    public TourEnquriyByIdSpec(DefaultIdType id) =>
        Query
            .Where(p => p.Id == id)
            .Include(x => x.Tour);
}