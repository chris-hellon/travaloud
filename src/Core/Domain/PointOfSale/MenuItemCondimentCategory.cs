using Travaloud.Domain.Stock;

namespace Travaloud.Domain.PointOfSale;

public abstract class MenuItemCondimentCategory(
        DefaultIdType menuItemId, 
        DefaultIdType categoryId,
        bool forcedOption,
        bool addOn,
        bool takeOff)
    : BaseEntity, IAggregateRoot
{
    public DefaultIdType MenuItemId { get; private set; } = menuItemId;
    
    public DefaultIdType CategoryId { get; private set; } = categoryId;
    
    public bool ForcedOption { get;private  set; } = forcedOption;
    
    public bool AddOn { get; private set; } = addOn;
    
    public bool TakeOff { get; private set; } = takeOff;

    public virtual Category Category { get; set; } = default!;
    public virtual IEnumerable<MenuItemCondimentCategoryMenuItem> CondimentCategoryMenuItems { get; set; } = default!;

    public MenuItemCondimentCategory Update(bool? forcedOption, bool? addOn, bool? takeOff)
    {
        if (forcedOption.HasValue && ForcedOption != forcedOption.Value)
            ForcedOption = forcedOption.Value;
        
        if (addOn.HasValue && AddOn != addOn.Value)
            AddOn = addOn.Value;
        
        if (takeOff.HasValue && TakeOff != takeOff.Value)
            TakeOff = takeOff.Value;

        return this;
    }
}