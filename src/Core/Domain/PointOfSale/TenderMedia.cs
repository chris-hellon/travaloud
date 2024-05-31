using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Domain.PointOfSale;

public abstract class TenderMedia(
        string tenderMediaType, 
        string name, 
        bool openCashDrawer, 
        bool reasonRequired,
        DefaultIdType siteId)
    : BaseEntity, IAggregateRoot
{
    [MaxLength(100)] 
    public string TenderMediaType { get; private set; } = tenderMediaType;
    
    [MaxLength(100)]  
    public string Name { get; private set; } = name;
    
    public bool OpenCashDrawer { get; private set; } = openCashDrawer;
    
    public bool ReasonRequired { get; private set; } = reasonRequired;
    
    public DefaultIdType PropertyId { get; set; } = siteId;

    public virtual Property Property { get; set; } = default!;
    
    public TenderMedia Update(string? tenderMediaType, string? name, bool? openCashDrawer, bool? reasonRequired)
    {
        if (tenderMediaType is not null && TenderMediaType != tenderMediaType)
            TenderMediaType = tenderMediaType;

        if (name is not null && Name != name)
            Name = name;

        if (openCashDrawer is not null && OpenCashDrawer != openCashDrawer)
            OpenCashDrawer = openCashDrawer.Value;

        if (reasonRequired is not null && ReasonRequired != reasonRequired)
            ReasonRequired = reasonRequired.Value;

        return this;
    }
}