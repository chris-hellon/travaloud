
namespace Travaloud.Domain.PointOfSale;

public abstract class DealPriceTier(
    DefaultIdType priceTierId, 
    DefaultIdType dealId, 
    decimal amount) 
    : BaseEntity, IAggregateRoot
{
    public DefaultIdType PriceTierId { get; private set; } = priceTierId;
    
    public DefaultIdType DealId { get; private set; } = dealId;
    
    [Precision(19, 4)] 
    public decimal Amount { get; private set; } = amount;

    public virtual PriceTier PriceTier { get; set; } = default!;

    public DealPriceTier Update(decimal? amount)
    {
        if (amount is not null && Amount != amount)
            Amount = amount.Value;

        return this;
    }
}
