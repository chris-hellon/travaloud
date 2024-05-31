namespace Travaloud.Domain.Stock;

public abstract class SupplierOfferReward(
        DefaultIdType supplierOfferId, 
        DefaultIdType productId, 
        string? description, 
        int quantity)
    : BaseEntity, IAggregateRoot
{
    private DefaultIdType SupplierOfferId { get; set; } = supplierOfferId;
    private DefaultIdType ProductId { get; set; } = productId;
    private string? Description { get; set; } = description;
    private int Quantity { get; set; } = quantity;

    public virtual SupplierOffer SupplierOffer { get; set; } = default!;
    public virtual Product Product { get; set; } = default!;

    
    public SupplierOfferReward Update(string? description, int? quantity)
    {
        if (description is not null && Description != description)
            Description = description;

        if (quantity is not null && Quantity != quantity)
            Quantity = quantity.Value;

        return this;
    }
}