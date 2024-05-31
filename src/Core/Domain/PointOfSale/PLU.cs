using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Domain.PointOfSale;

public abstract class PLU(
    string name, 
    string posName,
    DefaultIdType propertyId,
    bool? publishToUat,
    bool? publishToPos, 
    bool? publishToApp, 
    DefaultIdType? publishToUatAmendId, 
    DefaultIdType? publishToPosAmendId,
    DefaultIdType? publishToAppAmendId) 
    : PublishableEntity(publishToUat, publishToPos, publishToApp, publishToUatAmendId, publishToPosAmendId, publishToAppAmendId)
{
    [MaxLength(128)] 
    public string Name { get; private set; } = name;
    
    [MaxLength(256)] 
    public string POSName { get; private set; } = posName;
    
    public DefaultIdType PropertyId { get; private set; } = propertyId;

    public virtual Property Property { get; set; } = default!;
    
    public PLU Update(string? name,
        string? posName,
        bool? publishToUat,
        bool? publishToPos,
        bool? publishToApp,
        DefaultIdType? publishToUatAmendId,
        DefaultIdType? publishToPosAmendId,
        DefaultIdType? publishToAppAmendId)
    {
        if (name is not null && Name != name)
            Name = name;

        if (posName is not null && POSName != posName)
            POSName = posName;
        
        base.Update(publishToUat, publishToPos, publishToApp, publishToUatAmendId, publishToPosAmendId, publishToAppAmendId);

        return this;
    }
}