using System.Data;
using Travaloud.Application.Catalog.Properties.Dto;

namespace Travaloud.Application.Catalog.Properties.Queries;

public class GetPropertiesByDestinationRequest : IRequest<IEnumerable<PropertyDto>>
{
    public string TenantId { get; set; }
    public DefaultIdType DestinationId { get; set; }

    public GetPropertiesByDestinationRequest(string tenantId, DefaultIdType destinationId)
    {
        TenantId = tenantId;
        DestinationId = destinationId;
    }
}

internal class GetPropertiesByDestinationRequestHandler : IRequestHandler<GetPropertiesByDestinationRequest, IEnumerable<PropertyDto>>
{
    private readonly IDapperRepository _repository;

    public GetPropertiesByDestinationRequestHandler(IDapperRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PropertyDto>> Handle(GetPropertiesByDestinationRequest request, CancellationToken cancellationToken)
    {
        return await _repository.QueryAsync<PropertyDto>(
            sql: "GetDestinationProperties",
            param: new
            {
                request.TenantId,
                request.DestinationId
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);
    }
}