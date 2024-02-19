using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Domain.Catalog.Destinations;

public class DestinationTourCategoryLookup : BaseEntity, IAggregateRoot
{
    public DestinationTourCategoryLookup(DefaultIdType destinationId, DefaultIdType tourCategoryId)
    {
        DestinationId = destinationId;
        TourCategoryId = tourCategoryId;
    }

    public DefaultIdType DestinationId { get; private set; }
    public DefaultIdType TourCategoryId { get; private set; }

    public virtual Destination Destination { get; private set; } = default!;
    public virtual TourCategory TourCategory { get; private set; } = default!;
}