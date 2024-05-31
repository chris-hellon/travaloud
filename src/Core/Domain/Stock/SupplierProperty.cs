using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Domain.Stock;

public class SupplierProperty(DefaultIdType supplierId, DefaultIdType propertyId) : BaseEntity, IAggregateRoot
{
    private DefaultIdType SupplierId { get; set; } = supplierId;
    private DefaultIdType PropertyId { get; set; } = propertyId;

    public virtual Property Property { get; set; } = default!;
}