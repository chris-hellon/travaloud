namespace Travaloud.Domain.PointOfSale;

public abstract class MenuItemCondimentCategoryMenuItem(
        DefaultIdType menuItemId, 
        DefaultIdType menuItemCondimentCategoryId,
        int sortIndex)
    : BaseEntity, IAggregateRoot
{
    public DefaultIdType MenuItemId { get; private set; } = menuItemId;
    
    public DefaultIdType MenuItemCondimentCategoryId { get; private set; } = menuItemCondimentCategoryId;
    
    public int SortIndex { get; set; } = sortIndex;

    public virtual MenuItem MenuItem { get; set; } = default!;
    public virtual IEnumerable<MenuItemCondimentCategoryMenuItemModifier> Modifiers { get; set; } = default!;

    public MenuItemCondimentCategoryMenuItem Update(int? sortIndex)
    {
        if (sortIndex.HasValue && SortIndex != sortIndex.Value)
            SortIndex = sortIndex.Value;

        return this;
    }
}