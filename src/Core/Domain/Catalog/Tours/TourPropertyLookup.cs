using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Domain.Catalog.Tours;

public class TourPropertyLookup : BaseEntity, IAggregateRoot
{
    public TourPropertyLookup(DefaultIdType tourId, DefaultIdType propertyId)
    {
        TourId = tourId;
        PropertyId = propertyId;
    }

    public DefaultIdType TourId { get; private set; }
    public DefaultIdType PropertyId { get; private set; }

    public virtual Tour Tour { get; private set; } = default!;
    public virtual Property Property { get; private set; } = default!;
}