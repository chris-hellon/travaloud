using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Domain.Catalog.Tours;

public class TourPickupLocation : AuditableEntity, IAggregateRoot
{
    public DefaultIdType TourId { get; set; }
    public DefaultIdType PropertyId { get; set; }
    
    public virtual Property Property { get; set; }

    public TourPickupLocation(DefaultIdType tourId, DefaultIdType propertyId)
    {
        TourId = tourId;
        PropertyId = propertyId;
    }
}