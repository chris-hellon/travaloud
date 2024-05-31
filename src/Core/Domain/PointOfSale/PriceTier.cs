using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Domain.PointOfSale;

public abstract class PriceTier(
        string name,
        string? friendlyName,
        TimeSpan? mondayStart, TimeSpan? mondayEnd,
        TimeSpan? tuesdayStart, TimeSpan? tuesdayEnd,
        TimeSpan? wednesdayStart, TimeSpan? wednesdayEnd,
        TimeSpan? thursdayStart, TimeSpan? thursdayEnd,
        TimeSpan? fridayStart, TimeSpan? fridayEnd,
        TimeSpan? saturdayStart, TimeSpan? saturdayEnd,
        TimeSpan? sundayStart, TimeSpan? sundayEnd,
        DateTime? validFrom, DateTime? validTo,
        bool? defaultPriceTier,
        bool? publishToUat,
        bool? publishToPos,
        bool? publishToApp,
        DefaultIdType? publishToUatAmendId,
        DefaultIdType? publishToPosAmendId,
        DefaultIdType? publishToAppAmendId,
        DefaultIdType? propertyId)
    : PublishableEntity(publishToUat, publishToPos, publishToApp, publishToUatAmendId, publishToPosAmendId, publishToAppAmendId)
{
    [MaxLength(128)] 
    public string Name { get; private set; } = name;
    
    [MaxLength(128)] 
    public string? FriendlyName { get; private set; } = friendlyName;
    
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
    
    public DateTime? ValidFrom { get; private set; } = validFrom;
    
    public DateTime? ValidTo { get; private set; } = validTo;
    
    public bool? DefaultPriceTier { get; private set; } = defaultPriceTier;
    
    public DefaultIdType? PropertyId { get; private set; } = propertyId;

    public virtual Property Property { get; private set; } = default!;

    public PriceTier Update(
        string? name,
        string? friendlyName,
        TimeSpan? mondayStart, TimeSpan? mondayEnd,
        TimeSpan? tuesdayStart, TimeSpan? tuesdayEnd,
        TimeSpan? wednesdayStart, TimeSpan? wednesdayEnd,
        TimeSpan? thursdayStart, TimeSpan? thursdayEnd,
        TimeSpan? fridayStart, TimeSpan? fridayEnd,
        TimeSpan? saturdayStart, TimeSpan? saturdayEnd,
        TimeSpan? sundayStart, TimeSpan? sundayEnd,
        DateTime? validFrom, DateTime? validTo,
        bool? defaultPriceTier,
        bool? publishToUat, bool? publishToPos, bool? publishToApp,
        DefaultIdType? publishToUatAmendId, DefaultIdType? publishToPosAmendId, DefaultIdType? publishToAppAmendId)
    {
        if (name is not null && Name != name)
            Name = name;

        if (friendlyName is not null && FriendlyName != friendlyName)
            FriendlyName = friendlyName;

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

        if (validFrom is not null && ValidFrom != validFrom)
            ValidFrom = validFrom.Value;

        if (validTo is not null && ValidTo != validTo)
            ValidTo = validTo.Value;

        if (defaultPriceTier is not null && DefaultPriceTier != defaultPriceTier)
            DefaultPriceTier = defaultPriceTier.Value;

        base.Update(publishToUat, publishToPos, publishToApp, publishToUatAmendId, publishToPosAmendId, publishToAppAmendId);

        return this;
    }
}