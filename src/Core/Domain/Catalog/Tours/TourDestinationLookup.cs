using Travaloud.Domain.Catalog.Destinations;

namespace Travaloud.Domain.Catalog.Tours;

public class TourDestinationLookup : AuditableEntity, IAggregateRoot
{
    public TourDestinationLookup(DefaultIdType tourId, DefaultIdType destinationId)
    {
        TourId = tourId;
        DestinationId = destinationId;
    }

    public DefaultIdType TourId { get; private set; }
    public DefaultIdType DestinationId { get; private set; }

    public virtual Tour Tour { get; private set; } = default!;
    public virtual Destination Destination { get; private set; } = default!;
}