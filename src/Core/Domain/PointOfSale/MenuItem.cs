using Travaloud.Domain.Catalog.Properties;
using Travaloud.Domain.Stock;

namespace Travaloud.Domain.PointOfSale;

public abstract class MenuItem(DefaultIdType propertyId, string posName, string appName, string appDescription, decimal volume,
        decimal? vatRate,
        TimeSpan? mondayStart, TimeSpan? mondayEnd, TimeSpan? tuesdayStart, TimeSpan? tuesdayEnd,
        TimeSpan? wednesdayStart, TimeSpan? wednesdayEnd, TimeSpan? thursdayStart, TimeSpan? thursdayEnd,
        TimeSpan? fridayStart, TimeSpan? fridayEnd, TimeSpan? saturdayStart, TimeSpan? saturdayEnd,
        TimeSpan? sundayStart, TimeSpan? sundayEnd, string? imageUrl, bool? isClone, bool? publishToUat,
        bool? publishToPos, bool? publishToApp, DefaultIdType? publishToUatAmendId, DefaultIdType? publishToPosAmendId,
        DefaultIdType? publishToAppAmendId, DefaultIdType pluId, DefaultIdType? parentPluId, DefaultIdType unitId, bool isCondiment)
    : PublishableEntity(publishToUat, publishToPos, publishToApp, publishToUatAmendId, publishToPosAmendId, publishToAppAmendId)
{
    public DefaultIdType PropertyId { get; set; } = propertyId;
    
    [MaxLength(128)] 
    public string POSName { get; private set; } = posName;
    
    [MaxLength(128)] 
    public string AppName { get; private set; } = appName;
    
    [MaxLength(256)] 
    public string AppDescription { get; private set; } = appDescription;
    
    [Precision(7, 2)] 
    public decimal Volume { get; private set; } = volume;
    
    [Precision(10, 0)] 
    public decimal? VATRate { get; private set; } = vatRate;
    
    public TimeSpan? MondayStart { get; private set; } = mondayStart;
    
    public TimeSpan? MondayEnd { get; private set; } = mondayEnd;
    
    public TimeSpan? TuesdayStart { get; private set; } = tuesdayStart;
    
    public TimeSpan? TuesdayEnd { get; private set; } = tuesdayEnd;
    
    public TimeSpan? WednesdayStart { get; private set; } = wednesdayStart;
    
    public TimeSpan? WednesdayEnd { get; private set; } = wednesdayEnd;
    
    public TimeSpan? ThursdayStart { get; private set; } = thursdayStart;
    
    public TimeSpan? ThursdayEnd { get; private set; } = thursdayEnd;
    
    public TimeSpan? FridayStart { get; private set; } = fridayStart;
    
    public TimeSpan? FridayEnd { get; private set; } = fridayEnd;
    
    public TimeSpan? SaturdayStart { get; private set; } = saturdayStart;
    
    public TimeSpan? SaturdayEnd { get; private set; } = saturdayEnd;
    
    public TimeSpan? SundayStart { get; private set; } = sundayStart;
    
    public TimeSpan? SundayEnd { get; private set; } = sundayEnd;
    
    [MaxLength(500)] 
    public string? ImageUrl { get; private set; } = imageUrl;
    
    public bool? IsClone { get; private set; } = isClone;
    
    public DefaultIdType UnitId { get; private set; } = unitId;
    
    public DefaultIdType PLUId { get; private set; } = pluId;
    
    public DefaultIdType? ParentPLUId { get; private set; } = parentPluId;
    
    public bool IsCondiment { get; private set; } = isCondiment;

    public virtual Property Property { get; set; } = default!;
    public virtual Unit Unit { get; set; } = default!;
    public virtual PLU PLU { get; set; } = default!;
    public virtual PLU? ParentPLU { get; set; } = default!;
    public virtual IEnumerable<MenuItemPriceTier> PriceTiers { get; set; } = default!;
    public virtual IEnumerable<MenuItemModifier> Modifiers { get; set; } = default!;
    public virtual IEnumerable<MenuItemCategory> Categories { get; set; } = default!;
    public virtual IEnumerable<MenuItemIngredient> Ingredients { get; set; } = default!;
    public virtual IEnumerable<MenuItemCondimentCategory> ForcedOptionCategories { get; set; } = default!;
    public virtual IEnumerable<MenuItemCondimentCategory> AddOnCategories { get; set; } = default!;
    public virtual IEnumerable<MenuItemCondimentCategory> TakeOffCategories { get; set; } = default!;

    public MenuItem Update(
        string? posName,
        string? appName,
        string? appDescription,
        decimal? volume,
        decimal? vatRate,
        TimeSpan? mondayStart,
        TimeSpan? mondayEnd,
        TimeSpan? tuesdayStart,
        TimeSpan? tuesdayEnd,
        TimeSpan? wednesdayStart,
        TimeSpan? wednesdayEnd,
        TimeSpan? thursdayStart,
        TimeSpan? thursdayEnd,
        TimeSpan? fridayStart,
        TimeSpan? fridayEnd,
        TimeSpan? saturdayStart,
        TimeSpan? saturdayEnd,
        TimeSpan? sundayStart,
        TimeSpan? sundayEnd,
        string? imageUrl,
        bool? isClone,
        bool? publishToUat,
        bool? publishToPOS,
        bool? publishToApp,
        DefaultIdType? publishToUatAmendId,
        DefaultIdType? publishToPOSAmendId,
        DefaultIdType? publishToAppAmendId,
        DefaultIdType? pluId,
        DefaultIdType? parentPLUId,
        DefaultIdType? unitId,
        bool? isCondiment)
    {
        if (posName is not null && POSName != posName)
            POSName = posName;

        if (appName is not null && AppName != appName)
            AppName = appName;

        if (appDescription is not null && AppDescription != appDescription)
            AppDescription = appDescription;

        if (volume is not null && Volume != volume)
            Volume = volume.Value;

        if (vatRate is not null && VATRate != vatRate)
            VATRate = vatRate;

        if (mondayStart is not null && MondayStart != mondayStart)
            MondayStart = mondayStart.Value;

        if (mondayEnd is not null && MondayEnd != mondayEnd)
            MondayEnd = mondayEnd.Value;

        if (tuesdayStart is not null && TuesdayStart != tuesdayStart)
            TuesdayStart = tuesdayStart.Value;

        if (tuesdayEnd is not null && TuesdayEnd != tuesdayEnd)
            TuesdayEnd = tuesdayEnd.Value;

        if (wednesdayStart is not null && WednesdayStart != wednesdayStart)
            WednesdayStart = wednesdayStart.Value;

        if (wednesdayEnd is not null && WednesdayEnd != wednesdayEnd)
            WednesdayEnd = wednesdayEnd.Value;

        if (thursdayStart is not null && ThursdayStart != thursdayStart)
            ThursdayStart = thursdayStart.Value;

        if (thursdayEnd is not null && ThursdayEnd != thursdayEnd)
            ThursdayEnd = thursdayEnd.Value;

        if (fridayStart is not null && FridayStart != fridayStart)
            FridayStart = fridayStart.Value;

        if (fridayEnd is not null && FridayEnd != fridayEnd)
            FridayEnd = fridayEnd.Value;

        if (saturdayStart is not null && SaturdayStart != saturdayStart)
            SaturdayStart = saturdayStart.Value;

        if (saturdayEnd is not null && SaturdayEnd != saturdayEnd)
            SaturdayEnd = saturdayEnd.Value;

        if (sundayStart is not null && SundayStart != sundayStart)
            SundayStart = sundayStart.Value;

        if (sundayEnd is not null && SundayEnd != sundayEnd)
            SundayEnd = sundayEnd.Value;

        if (imageUrl is not null && ImageUrl != imageUrl)
            ImageUrl = imageUrl;

        if (isClone is not null && IsClone != isClone)
            IsClone = isClone;

        if (pluId is not null && PLUId != pluId)
            PLUId = pluId.Value;

        if (parentPLUId is not null && ParentPLUId != parentPLUId)
            ParentPLUId = parentPLUId;

        if (unitId is not null && UnitId != unitId)
            UnitId = unitId.Value;

        if (isCondiment is not null && IsCondiment != isCondiment)
            IsCondiment = isCondiment.Value;

        base.Update(publishToUat, publishToPos, publishToApp, publishToUatAmendId, publishToPosAmendId, publishToAppAmendId);

        return this;
    }
}