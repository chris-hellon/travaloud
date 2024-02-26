using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Application.Catalog.Properties.Specification;
using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Application.Catalog.Properties.Queries;

public class GetPropertiesByPublishToWebsiteRequest : IRequest<IEnumerable<PropertyDto>>
{
   
}

public class GetPropertiesByPublishToWebsiteRequestHandler : IRequestHandler<GetPropertiesByPublishToWebsiteRequest, IEnumerable<PropertyDto>>
{
    private readonly IRepositoryFactory<Property> _repository;

    public GetPropertiesByPublishToWebsiteRequestHandler(IRepositoryFactory<Property> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PropertyDto>> Handle(GetPropertiesByPublishToWebsiteRequest request, CancellationToken cancellationToken)
    {
        var spec = new PropertiesByPublishToWebsiteSpec(request);

        return await _repository.ListAsync(spec, cancellationToken: cancellationToken);
    }
}