using Travaloud.Application.Catalog.ServicesEnquiries.Dto;
using Travaloud.Application.Catalog.ServicesEnquiries.Specification;
using Travaloud.Domain.Catalog.Enquiries;

namespace Travaloud.Application.Catalog.ServicesEnquiries.Queries;

public class GetServiceEnquiryRequest : IRequest<ServiceEnquiryDetailsDto>
{
    public GetServiceEnquiryRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class GetServiceEnquiryRequestHandler : IRequestHandler<GetServiceEnquiryRequest, ServiceEnquiryDetailsDto>
{
    private readonly IRepositoryFactory<ServiceEnquiry> _repository;
    private readonly IStringLocalizer<GetServiceEnquiryRequestHandler> _localizer;

    public GetServiceEnquiryRequestHandler(IRepositoryFactory<ServiceEnquiry> repository,
        IStringLocalizer<GetServiceEnquiryRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<ServiceEnquiryDetailsDto> Handle(GetServiceEnquiryRequest request, CancellationToken cancellationToken) =>
        await _repository.FirstOrDefaultAsync(
            new ServiceEnquriyByIdSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(_localizer["serviceenquiry.notfound"], request.Id));
}