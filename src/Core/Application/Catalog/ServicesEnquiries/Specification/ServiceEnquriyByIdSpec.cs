using Travaloud.Application.Catalog.ServicesEnquiries.Dto;
using Travaloud.Domain.Catalog.Enquiries;

namespace Travaloud.Application.Catalog.ServicesEnquiries.Specification;

public class ServiceEnquriyByIdSpec : Specification<ServiceEnquiry, ServiceEnquiryDetailsDto>, ISingleResultSpecification<ServiceEnquiry>
{
    public ServiceEnquriyByIdSpec(DefaultIdType id) =>
        Query
            .Where(p => p.Id == id)
            .Include(x => x.Service).
            Include(x => x.Fields);
}