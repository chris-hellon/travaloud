using Travaloud.Application.Catalog.Services.Dto;
using Travaloud.Application.Catalog.Services.Specification;
using Travaloud.Domain.Catalog.Services;

namespace Travaloud.Application.Catalog.Services.Queries;

public class GetServicesRequest : IRequest<IEnumerable<ServiceDto>>
{
    public string? Name { get; set; }
}

public class GetServicesRequestHandler : IRequestHandler<GetServicesRequest, IEnumerable<ServiceDto>>
{
    private readonly IRepositoryFactory<Service> _repository;

    public GetServicesRequestHandler(IRepositoryFactory<Service> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ServiceDto>> Handle(GetServicesRequest request, CancellationToken cancellationToken)
    {
        var spec = new GetServicesSpec(request);
        return await _repository.ListAsync(spec, cancellationToken: cancellationToken);
    }
}