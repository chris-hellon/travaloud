using Travaloud.Application.Catalog.ServicesEnquiries.Dto;
using Travaloud.Domain.Catalog.Enquiries;

namespace Travaloud.Application.Catalog.ServicesEnquiries.Queries;

public class SearchServiceEnquiriesRequest : PaginationFilter, IRequest<PaginationResponse<ServiceEnquiryDto>>
{
    public string? Name { get; set; }
}

public class SearchServiceEnquiriesRequestHandler : IRequestHandler<SearchServiceEnquiriesRequest, PaginationResponse<ServiceEnquiryDto>>
{
    private readonly IRepositoryFactory<ServiceEnquiry> _repository;

    public SearchServiceEnquiriesRequestHandler(IRepositoryFactory<ServiceEnquiry> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<ServiceEnquiryDto>> Handle(SearchServiceEnquiriesRequest request, CancellationToken cancellationToken)
    {
        var spec = new ServiceEnquiriesBySearchRequest(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}