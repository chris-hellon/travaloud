using Travaloud.Application.Catalog.TourEnquiries.Dto;
using Travaloud.Application.Catalog.TourEnquiries.Specification;
using Travaloud.Domain.Catalog.Enquiries;

namespace Travaloud.Application.Catalog.TourEnquiries.Queries;

public class GetTourEnquiryRequest : IRequest<TourEnquiryDto>
{
    public GetTourEnquiryRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class GetTourEnquiryRequestHandler : IRequestHandler<GetTourEnquiryRequest, TourEnquiryDto>
{
    private readonly IRepositoryFactory<TourEnquiry> _repository;
    private readonly IStringLocalizer<GetTourEnquiryRequestHandler> _localizer;

    public GetTourEnquiryRequestHandler(IRepositoryFactory<TourEnquiry> repository,
        IStringLocalizer<GetTourEnquiryRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<TourEnquiryDto> Handle(GetTourEnquiryRequest request, CancellationToken cancellationToken) =>
        await _repository.FirstOrDefaultAsync(
            new TourEnquriyByIdSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(_localizer["tourenquiry.notfound"], request.Id));
}