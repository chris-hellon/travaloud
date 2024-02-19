using Travaloud.Application.Catalog.TourEnquiries.Dto;
using Travaloud.Application.Catalog.TourEnquiries.Specification;
using Travaloud.Domain.Catalog.Enquiries;

namespace Travaloud.Application.Catalog.TourEnquiries.Queries;

public class SearchTourEnquiriesRequest : PaginationFilter, IRequest<PaginationResponse<TourEnquiryDto>>
{
    public string? Name { get; set; }
}

public class SearchTourEnquiriesRequestHandler : IRequestHandler<SearchTourEnquiriesRequest, PaginationResponse<TourEnquiryDto>>
{
    private readonly IRepositoryFactory<TourEnquiry> _repository;

    public SearchTourEnquiriesRequestHandler(IRepositoryFactory<TourEnquiry> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<TourEnquiryDto>> Handle(SearchTourEnquiriesRequest request, CancellationToken cancellationToken)
    {
        var spec = new TourEnquiriesBySearchSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}