using Travaloud.Domain.Stock;

namespace Travaloud.Domain.PointOfSale;

public abstract class MenuItemIngredient(
        DefaultIdType menuItemId, 
        DefaultIdType productId,
        DefaultIdType unitId, 
        decimal volume, 
        string? color,
        int? sortOrder)
    : BaseEntity, IAggregateRoot
{
    public DefaultIdType MenuItemId { get; private set; } = menuItemId;
    
    public DefaultIdType ProductId { get; private set; } = productId;
    
    public DefaultIdType UnitId { get; private set; } = unitId;
    
    [Precision(7, 2)] 
    public decimal Volume { get; private set; } = volume;
    
    [MaxLength(50)] 
    public string? Color { get; private set; } = color;
    
    public int? SortOrder { get; private set; } = sortOrder;

    public virtual Product Product { get; set; } = default!;
    public virtual Unit Unit { get; set; } = default!;
    
    public MenuItemIngredient Update(DefaultIdType? unitId, decimal? volume, string? color, int? sortOrder)
    {
        if (unitId is not null && UnitId != unitId)
            UnitId = unitId.Value;

        if (volume is not null && Volume != volume)
            Volume = volume.Value;

        if (color is not null && Color != color)
            Color = color;

        if (sortOrder is not null && SortOrder != sortOrder)
            SortOrder = sortOrder.Value;

        return this;
    }
}