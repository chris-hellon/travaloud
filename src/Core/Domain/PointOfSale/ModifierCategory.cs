using Travaloud.Domain.Stock;

namespace Travaloud.Domain.PointOfSale;

public abstract class ModifierCategory(
    DefaultIdType modifierId, 
    DefaultIdType categoryId) 
    : AuditableEntity, IAggregateRoot
{
    public DefaultIdType ModifierId { get; private set; } = modifierId;
    
    public DefaultIdType CategoryId { get; private set; } = categoryId;
    
    public virtual Category Category { get; set; } = default!;
}