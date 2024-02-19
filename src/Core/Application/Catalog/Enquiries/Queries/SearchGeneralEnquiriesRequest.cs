using Travaloud.Application.Catalog.Enquiries.Dto;
using Travaloud.Application.Catalog.Enquiries.Specification;
using Travaloud.Domain.Catalog.Enquiries;

namespace Travaloud.Application.Catalog.Enquiries.Queries;

public class SearchGeneralEnquiriesRequest : PaginationFilter, IRequest<PaginationResponse<GeneralEnquiryDto>>
{
    public string? Name { get; set; }
}

public class SearchGeneralEnquiriesRequestHandler : IRequestHandler<SearchGeneralEnquiriesRequest, PaginationResponse<GeneralEnquiryDto>>
{
    private readonly IRepositoryFactory<GeneralEnquiry> _repository;

    public SearchGeneralEnquiriesRequestHandler(IRepositoryFactory<GeneralEnquiry> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<GeneralEnquiryDto>> Handle(SearchGeneralEnquiriesRequest request, CancellationToken cancellationToken)
    {
        var spec = new GeneralEnquiriesBySearchSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}