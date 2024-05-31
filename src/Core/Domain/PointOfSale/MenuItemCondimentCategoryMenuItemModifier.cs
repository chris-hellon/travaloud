namespace Travaloud.Domain.PointOfSale;

public abstract class MenuItemCondimentCategoryMenuItemModifier(
        DefaultIdType menuItemCondimentCategoryMenuItemId,
        DefaultIdType modifierId)
    : BaseEntity, IAggregateRoot
{
    public DefaultIdType MenuItemCondimentCategoryMenuItemId { get; private set; } = menuItemCondimentCategoryMenuItemId;
    
    public DefaultIdType ModifierId { get; private set; } = modifierId;

    public virtual Modifier Modifier { get; set; } = default!;
}
