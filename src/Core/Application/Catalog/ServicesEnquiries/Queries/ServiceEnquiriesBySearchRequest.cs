using Travaloud.Application.Catalog.ServicesEnquiries.Dto;
using Travaloud.Domain.Catalog.Enquiries;

namespace Travaloud.Application.Catalog.ServicesEnquiries.Queries;

public class ServiceEnquiriesBySearchRequest : EntitiesByPaginationFilterSpec<ServiceEnquiry, ServiceEnquiryDto>
{
    public ServiceEnquiriesBySearchRequest(SearchServiceEnquiriesRequest request)
        : base(request) =>
        Query
            .OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
            .Where(c => c.Service.Title.Equals(request.Name), request.Name != null);
}