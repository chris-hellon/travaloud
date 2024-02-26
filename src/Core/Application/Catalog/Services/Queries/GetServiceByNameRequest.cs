using Travaloud.Application.Catalog.Services.Dto;
using Travaloud.Application.Catalog.Services.Specification;
using Travaloud.Domain.Catalog.Services;

namespace Travaloud.Application.Catalog.Services.Queries;

public class GetServiceByNameRequest : IRequest<ServiceDetailsDto>
{
    public string ServiceName { get; set; }
    public GetServiceByNameRequest(string serviceName) => ServiceName = serviceName;
}

public class GetServiceByNameRequestHandler : IRequestHandler<GetServiceByNameRequest, ServiceDetailsDto>
{
    private readonly IRepositoryFactory<Service> _repository;
    private readonly IStringLocalizer<GetServiceByNameRequestHandler> _localizer;

    public GetServiceByNameRequestHandler(IRepositoryFactory<Service> repository,
        IStringLocalizer<GetServiceByNameRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<ServiceDetailsDto> Handle(GetServiceByNameRequest request, CancellationToken cancellationToken) =>
        await _repository.FirstOrDefaultAsync(
            new ServiceByFriendlyNameSpec(request.ServiceName), cancellationToken)
        ?? throw new NotFoundException(string.Format(_localizer["service.notfound"], request.ServiceName));
}