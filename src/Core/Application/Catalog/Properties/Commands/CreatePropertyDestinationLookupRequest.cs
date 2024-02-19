namespace Travaloud.Application.Catalog.Properties.Commands;

public class CreatePropertyDestinationLookupRequest : IRequest<DefaultIdType>
{
    public DefaultIdType DestinationId { get; set; }
}