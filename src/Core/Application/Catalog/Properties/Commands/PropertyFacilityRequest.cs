namespace Travaloud.Application.Catalog.Properties.Commands;

public class PropertyFacilityRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType PropertyId { get; set; }
    public string Title { get; set; } = default!;
}