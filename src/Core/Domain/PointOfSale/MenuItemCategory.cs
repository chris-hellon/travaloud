using Travaloud.Domain.Stock;

namespace Travaloud.Domain.PointOfSale;

public abstract class MenuItemCategory(DefaultIdType menuItemId, DefaultIdType categoryId) 
    : BaseEntity, IAggregateRoot
{
    public DefaultIdType MenuItemId { get; private set; } = menuItemId;
    
    public DefaultIdType CategoryId { get; private set; } = categoryId;

    public virtual Category Category { get; set; } = default!;
}