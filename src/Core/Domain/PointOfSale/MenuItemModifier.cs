namespace Travaloud.Domain.PointOfSale;

public abstract class MenuItemModifier(
    DefaultIdType menuItemId, 
    DefaultIdType modifierId) 
    : BaseEntity, IAggregateRoot
{
    public DefaultIdType MenuItemId { get; private set; } = menuItemId;
    
    public DefaultIdType ModifierId { get; private set; } = modifierId;

    public virtual Modifier Modifier { get; set; } = default!;
    public virtual IEnumerable<MenuItemModifierPriceTier> ModifierPrices { get; set; } = default!;
}