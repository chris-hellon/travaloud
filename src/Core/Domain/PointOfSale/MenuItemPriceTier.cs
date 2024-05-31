namespace Travaloud.Domain.PointOfSale;

public abstract class MenuItemPriceTier(
        DefaultIdType priceTierId, 
        DefaultIdType menuItemId)
    : BaseEntity, IAggregateRoot
{
    public DefaultIdType PriceTierId { get; private set; } = priceTierId;
    
    public DefaultIdType MenuItemId { get; private set; } = menuItemId;

    public virtual PriceTier PriceTier { get; set; } = default!;
}