using Travaloud.Application.Catalog.Enquiries.Dto;
using Travaloud.Application.Catalog.Enquiries.Specification;
using Travaloud.Domain.Catalog.Enquiries;

namespace Travaloud.Application.Catalog.Enquiries.Queries;

public class GetGeneralEnquiryRequest : IRequest<GeneralEnquiryDto>
{
    public GetGeneralEnquiryRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class GetGeneralEnquiryRequestHandler : IRequestHandler<GetGeneralEnquiryRequest, GeneralEnquiryDto>
{
    private readonly IRepositoryFactory<GeneralEnquiry> _repository;
    private readonly IStringLocalizer<GetGeneralEnquiryRequestHandler> _localizer;

    public GetGeneralEnquiryRequestHandler(IRepositoryFactory<GeneralEnquiry> repository,
        IStringLocalizer<GetGeneralEnquiryRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<GeneralEnquiryDto> Handle(GetGeneralEnquiryRequest request, CancellationToken cancellationToken) =>
        await _repository.FirstOrDefaultAsync(
            new GeneralEnquriyByIdSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(_localizer["generalenquiry.notfound"], request.Id));
}