using Travaloud.Application.Catalog.Enquiries.Dto;
using Travaloud.Domain.Catalog.Enquiries;

namespace Travaloud.Application.Catalog.Enquiries.Specification;

public class GeneralEnquriyByIdSpec : Specification<GeneralEnquiry, GeneralEnquiryDto>, ISingleResultSpecification<GeneralEnquiry>
{
    public GeneralEnquriyByIdSpec(DefaultIdType id) =>
        Query
            .Where(p => p.Id == id);
}