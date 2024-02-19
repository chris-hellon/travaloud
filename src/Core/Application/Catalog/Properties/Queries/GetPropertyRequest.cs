using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Application.Catalog.Properties.Specification;
using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Application.Catalog.Properties.Queries;

public class GetPropertyRequest : IRequest<PropertyDetailsDto>
{
    public GetPropertyRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class GetPropertyRequestHandler : IRequestHandler<GetPropertyRequest, PropertyDetailsDto>
{
    private readonly IRepositoryFactory<Property> _repository;
    private readonly IStringLocalizer<GetPropertyRequestHandler> _localizer;

    public GetPropertyRequestHandler(IRepositoryFactory<Property> repository,
        IStringLocalizer<GetPropertyRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<PropertyDetailsDto> Handle(GetPropertyRequest request, CancellationToken cancellationToken) =>
        await _repository.FirstOrDefaultAsync(
            new PropertyByIdSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(_localizer["property.notfound"], request.Id));
}