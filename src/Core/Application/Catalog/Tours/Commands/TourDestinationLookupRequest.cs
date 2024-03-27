namespace Travaloud.Application.Catalog.Tours.Commands;

public class TourDestinationLookupRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType TourId { get; set; }
    public DefaultIdType DestinationId { get; set; }
    public string Name { get; set; } = default!;
    
    public TourDestinationLookupRequest()
    {
        
    }
    
    public TourDestinationLookupRequest(DefaultIdType tourId, DefaultIdType destinationId, string name)
    {
        TourId = tourId;
        DestinationId = destinationId;
        Name = name;
    }
    
    public override int GetHashCode() => Name?.GetHashCode() ?? 0;
    
    public override string ToString() => Name;
    
    public override bool Equals(object o)
    {
        var other = o as TourDestinationLookupRequest;
        return other?.Name == Name;
    }
}