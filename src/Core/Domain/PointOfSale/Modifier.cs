namespace Travaloud.Domain.PointOfSale;

public abstract class Modifier(
        string posName, 
        string appName, 
        string? appDescription, 
        decimal multiplier, 
        bool? publishToUat,
        bool? publishToPos, 
        bool? publishToApp, 
        DefaultIdType? publishToUatAmendId, 
        DefaultIdType? publishToPosAmendId,
        DefaultIdType? publishToAppAmendId)
    : PublishableEntity(publishToUat, publishToPos, publishToApp, publishToUatAmendId, publishToPosAmendId, publishToAppAmendId)
{
    [MaxLength(128)] 
    public string POSName { get; private set; } = posName;
    
    [MaxLength(128)] 
    public string AppName { get; private set; } = appName;
    
    [MaxLength(256)] 
    public string? AppDescription { get; private set; } = appDescription; 
    
    [Precision(7, 4)] 
    public decimal Multiplier { get; private set; } = multiplier;

    public virtual IEnumerable<ModifierCategory> ModifierCategories { get; set; } = default!;

    public Modifier Update(string? posName, string? appName, string? appDescription, decimal? multiplier, bool? publishToUat, bool? publishToPos, bool? publishToApp, DefaultIdType? publishToUatAmendId, DefaultIdType? publishToPosAmendId, DefaultIdType? publishToAppAmendId)
    {
        if (posName is not null && POSName != posName)
            POSName = posName;

        if (appName is not null && AppName != appName)
            AppName = appName;

        if (appDescription is not null && AppDescription != appDescription)
            AppDescription = appDescription;

        if (multiplier is not null && Multiplier != multiplier)
            Multiplier = multiplier.Value;

        base.Update(publishToUat, publishToPos, publishToApp, publishToUatAmendId, publishToPosAmendId, publishToAppAmendId);

        return this;
    }
}