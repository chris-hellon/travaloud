namespace Travaloud.Application.Catalog.Properties.Commands;

public class PropertyDestinationLookupRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType PropertyId { get; set; }
    public DefaultIdType DestinationId { get; set; }
    public string Name { get; set; } = default!;

    public PropertyDestinationLookupRequest()
    {
        
    }
    
    public PropertyDestinationLookupRequest(DefaultIdType propertyId, DefaultIdType destinationId, string name)
    {
        PropertyId = propertyId;
        DestinationId = destinationId;
        Name = name;
    }
    
    public override int GetHashCode() => Name?.GetHashCode() ?? 0;
    
    public override string ToString() => Name;
    
    public override bool Equals(object? o)
    {
        var other = o as PropertyDestinationLookupRequest;
        return other?.Name == Name;
    }
}