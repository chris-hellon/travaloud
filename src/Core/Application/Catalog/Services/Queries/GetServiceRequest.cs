using Travaloud.Application.Catalog.Services.Dto;
using Travaloud.Application.Catalog.Services.Specification;
using Travaloud.Domain.Catalog.Services;

namespace Travaloud.Application.Catalog.Services.Queries;

public class GetServiceRequest : IRequest<ServiceDetailsDto>
{
    public GetServiceRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class GetServiceRequestHandler : IRequestHandler<GetServiceRequest, ServiceDetailsDto>
{
    private readonly IRepositoryFactory<Service> _repository;
    private readonly IStringLocalizer<GetServiceRequestHandler> _localizer;

    public GetServiceRequestHandler(IRepositoryFactory<Service> repository,
        IStringLocalizer<GetServiceRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<ServiceDetailsDto> Handle(GetServiceRequest request, CancellationToken cancellationToken) =>
        await _repository.FirstOrDefaultAsync(
            new ServiceByIdSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(_localizer["service.notfound"], request.Id));
}