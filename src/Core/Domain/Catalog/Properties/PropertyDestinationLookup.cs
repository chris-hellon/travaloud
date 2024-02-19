using Travaloud.Domain.Catalog.Destinations;

namespace Travaloud.Domain.Catalog.Properties;

public class PropertyDestinationLookup : AuditableEntity, IAggregateRoot
{
    public PropertyDestinationLookup(DefaultIdType destinationId)
    {
        DestinationId = destinationId;
    }

    public DefaultIdType PropertyId { get; private set; }
    public DefaultIdType DestinationId { get; private set; }

    public virtual Property Property { get; private set; } = default!;
    public virtual Destination Destination { get; private set; } = default!;
}