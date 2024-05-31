using Travaloud.Domain.PointOfSale;

namespace Travaloud.Domain.Stock;

public abstract class Category(
        string name, 
        string? posName, 
        string? appName, 
        string? appDescription, 
        bool? publishToUat,
        bool? publishToApp, 
        bool? publishToPos, 
        DefaultIdType categoryTypeId,
        DefaultIdType? publishToUatAmendId, 
        DefaultIdType? publishToAppAmendId,
        DefaultIdType? publishToPosAmendId, 
        DefaultIdType? parentCategoryId)
    : PublishableEntity(publishToUat, publishToPos, publishToApp, publishToUatAmendId, publishToPosAmendId, publishToAppAmendId)
{
    [MaxLength(128)] 
    public string Name { get; private set; } = name;
    
    [MaxLength(128)] 
    public string? POSName { get; private set; } = posName;
    
    [MaxLength(128)] 
    public string? AppName { get; private set; } = appName;
    
    [MaxLength(256)] 
    public string? AppDescription { get; private set; } = appDescription;
    
    private DefaultIdType CategoryTypeId { get; set; } = categoryTypeId;
    
    public DefaultIdType? ParentCategoryId { get; private set; } = parentCategoryId;

    public virtual CategoryType CategoryType { get; set; } = default!;
    
    public virtual Category? ParentCategory { get; set; }
    
    public Category Update(
        string? name,
        string? posName,
        string? appName,
        string? appDescription,
        bool? publishToUat,
        bool? publishToApp,
        bool? publishToPos,
        DefaultIdType? publishToUatAmendId,
        DefaultIdType? publishToAppAmendId,
        DefaultIdType? publishToPosAmendId,
        DefaultIdType? parentCategoryId)
    {
        if (name is not null && Name != name)
            Name = name;

        if (posName is not null && POSName != posName)
            POSName = posName;

        if (appName is not null && AppName != appName)
            AppName = appName;

        if (appDescription is not null && AppDescription != appDescription)
            AppDescription = appDescription;

        base.Update(publishToUat, publishToPos, publishToApp, publishToUatAmendId, publishToPosAmendId, publishToAppAmendId);
        
        if (parentCategoryId is not null && ParentCategoryId != parentCategoryId)
            ParentCategoryId = parentCategoryId.Value;

        return this;
    }
}