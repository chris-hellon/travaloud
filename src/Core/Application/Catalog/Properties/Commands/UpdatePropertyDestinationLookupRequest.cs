namespace Travaloud.Application.Catalog.Properties.Commands;

public class UpdatePropertyDestinationLookupRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType DestinationId { get; set; }
}