using Travaloud.Domain.Catalog.Properties;
using Travaloud.Domain.Identity;

namespace Travaloud.Domain.Stock;

public abstract class Return(
        DefaultIdType propertyId, 
        DefaultIdType productId, 
        int quantity, 
        decimal volume, 
        DefaultIdType unitId, 
        DefaultIdType categoryId,
        DefaultIdType supplierId, 
        DefaultIdType reasonId)
    : AuditableEntity, IAggregateRoot
{
    private DefaultIdType PropertyId { get; set; } = propertyId;
    private DefaultIdType ProductId { get; set; } = productId;
    private int Quantity { get; set; } = quantity;
    private decimal Volume { get; set; } = volume;
    private DefaultIdType UnitId { get; set; } = unitId;
    private DefaultIdType CategoryId { get; set; } = categoryId;
    private DefaultIdType SupplierId { get; set; } = supplierId;
    private DefaultIdType ReasonId { get; set; } = reasonId;

    public virtual Property Property { get; set; } = default!;
    public virtual Product Product { get; set; } = default!;
    public virtual Unit Unit { get; set; } = default!;
    public virtual Category Category { get; set; } = default!;
    public virtual ApplicationUser Supplier { get; set; } = default!;
    public virtual Reason Reason { get; set; } = default!;

    public Return Update(int quantity, decimal volume, DefaultIdType unitId, DefaultIdType categoryId, DefaultIdType supplierId, DefaultIdType reasonId)
    {
        if (quantity != Quantity)
            Quantity = quantity;

        if (volume != Volume)
            Volume = volume;

        if (unitId != UnitId)
            UnitId = unitId;

        if (categoryId != CategoryId)
            CategoryId = categoryId;

        if (supplierId != SupplierId)
            SupplierId = supplierId;

        if (reasonId != ReasonId)
            ReasonId = reasonId;

        return this;
    }
}