using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Domain.PointOfSale;

public abstract class PinEntryDevice(
    int terminalNumber, 
    DefaultIdType propertyId) : BaseEntity, IAggregateRoot
{
    [MaxLength(128)] 
    public int TerminalNumber { get; private set; } = terminalNumber;
    
    public DefaultIdType PropertyId { get; private set; } = propertyId;

    public virtual Property Property { get; set; } = default!;

    public PinEntryDevice Update(int? terminalNumber, DefaultIdType? propertyId)
    {
        if (terminalNumber is not null && TerminalNumber != terminalNumber)
            TerminalNumber = terminalNumber.Value;

        if (propertyId is not null && PropertyId != propertyId)
            PropertyId = propertyId.Value;

        return this;
    }
}