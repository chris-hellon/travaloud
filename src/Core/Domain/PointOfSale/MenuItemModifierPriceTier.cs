namespace Travaloud.Domain.PointOfSale;

public abstract class MenuItemModifierPriceTier(
        DefaultIdType priceTierId, 
        DefaultIdType menuItemModifierId,
        decimal amount)
    : BaseEntity, IAggregateRoot
{
    public DefaultIdType PriceTierId { get; private set; } = priceTierId;
    
    public DefaultIdType MenuItemModifierId { get; private set; } = menuItemModifierId;
    
    [Precision(19,4)] 
    public decimal Amount { get; private set; } = amount;

    public virtual PriceTier PriceTier { get; set; } = default!;

    public MenuItemModifierPriceTier Update(decimal? amount)
    {
        if (amount is not null && Amount != amount)
            Amount = amount.Value;

        return this;
    }
}