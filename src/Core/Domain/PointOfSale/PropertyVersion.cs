using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Domain.PointOfSale;

public abstract class PropertyVersion(DefaultIdType propertyId, decimal version, string posDataJson) : AuditableEntity, IAggregateRoot
{
    public DefaultIdType PropertyId { get; private set; } = propertyId;
    
    [Precision(5, 2)] 
    public decimal Version { get; private set; } = version;
    
    public string POSDataJson { get; private set; } = posDataJson;

    public virtual Property Property { get; set; } = default!;

    public PropertyVersion Update(decimal? version, string? posDataJson)
    {
        if (version is not null && Version != version.Value)
            Version = version.Value;

        if (posDataJson is not null && POSDataJson != posDataJson)
            POSDataJson = posDataJson;
        
        return this;
    }
}