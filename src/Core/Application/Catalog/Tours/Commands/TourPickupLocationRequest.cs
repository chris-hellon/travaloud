using Travaloud.Application.Catalog.Properties.Dto;

namespace Travaloud.Application.Catalog.Tours.Commands;

public class TourPickupLocationRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType TourId { get; set; }
    public DefaultIdType PropertyId { get; set; }
    public string? PropertyName { get; set; }

    public TourPickupLocationRequest()
    {
    }

    public TourPickupLocationRequest(DefaultIdType tourId, DefaultIdType propertyId, string? propertyName)
    {
        TourId = tourId;
        PropertyId = propertyId;
        PropertyName = propertyName;
    }

    public TourPickupLocationRequest(DefaultIdType id, DefaultIdType tourId, DefaultIdType propertyId, string? propertyName)
    {
        Id = id;
        TourId = tourId;
        PropertyId = propertyId;
        PropertyName = propertyName;
    }

    public override int GetHashCode() => PropertyName?.GetHashCode() ?? 0;
    
    public override string ToString() => PropertyName;
    
    public override bool Equals(object? o)
    {
        var other = o as TourPickupLocationRequest;
        return other?.PropertyName == PropertyName;
    }
}