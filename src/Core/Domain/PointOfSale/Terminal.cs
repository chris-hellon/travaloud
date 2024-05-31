using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Domain.PointOfSale;

public abstract class Terminal(
        string workstationType, 
        int terminalNumber, 
        bool hasCashDrawer,
        DefaultIdType propertyId)
    : BaseEntity, IAggregateRoot
{
    [MaxLength(50)] 
    public string WorkstationType { get; private set; } = workstationType;
    
    public int TerminalNumber { get; private set; } = terminalNumber;
    
    public bool HasCashDrawer { get; private set; } = hasCashDrawer;
    
    public DefaultIdType PropertyId { get; private set; } = propertyId;

    public virtual Property Property { get; set; } = default!;
    
    public Terminal Update(string? workstationType, int? terminalNumber, bool? hasCashDrawer)
    {
        if (workstationType is not null && WorkstationType != workstationType)
            WorkstationType = workstationType;

        if (terminalNumber is not null && TerminalNumber != terminalNumber)
            TerminalNumber = terminalNumber.Value;

        if (hasCashDrawer is not null && HasCashDrawer != hasCashDrawer)
            HasCashDrawer = hasCashDrawer.Value;

        return this;
    }
}