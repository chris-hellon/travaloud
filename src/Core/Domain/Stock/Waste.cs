using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Domain.Stock;

public abstract class Waste(
        DefaultIdType propertyId, 
        DefaultIdType productId, 
        int quantity, 
        decimal volume, 
        DefaultIdType unitId, 
        DefaultIdType categoryId,
        DefaultIdType reasonId, 
        DateTime date)
    : AuditableEntity, IAggregateRoot
{
    private DefaultIdType PropertyId { get; set; } = propertyId;
    private DefaultIdType ProductId { get; set; } = productId;
    private int Quantity { get; set; } = quantity;
    private decimal Volume { get; set; } = volume;
    private DefaultIdType UnitId { get; set; } = unitId;
    private DefaultIdType CategoryId { get; set; } = categoryId;
    private DefaultIdType ReasonId { get; set; } = reasonId;
    private DateTime Date { get; set; } = date;

    public virtual Property Property { get; set; } = default!;
    public virtual Product Product { get; set; } = default!;
    public virtual Unit Unit { get; set; } = default!;
    public virtual Category Category { get; set; } = default!;
    public virtual Reason Reason { get; set; } = default!;

    public Waste Update(DefaultIdType? productId, int? quantity, decimal? volume, DefaultIdType? unitId, DefaultIdType? categoryId, DefaultIdType? reasonId, DateTime? date)
    {
        if (productId is not null && ProductId != productId)
            ProductId = productId.Value;

        if (quantity is not null && Quantity != quantity)
            Quantity = quantity.Value;

        if (volume is not null && Volume != volume)
            Volume = volume.Value;

        if (unitId is not null && UnitId != unitId)
            UnitId = unitId.Value;

        if (categoryId is not null && CategoryId != categoryId)
            CategoryId = categoryId.Value;

        if (reasonId is not null && ReasonId != reasonId)
            ReasonId = reasonId.Value;

        if (date is not null && Date != date)
            Date = date.Value;

        return this;
    }
}