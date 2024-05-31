using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Domain.PointOfSale;

public abstract class Session(
        DateTime startDate, 
        DateTime? endDate, 
        DefaultIdType propertyId,
        DefaultIdType terminalId)
    : BaseEntity, IAggregateRoot
{
    public DateTime StartDate { get; private set; } = startDate;
    
    public DateTime? EndDate { get; private set; } = endDate;
    
    public DefaultIdType PropertyId { get; private set; } = propertyId;
    
    public DefaultIdType TerminalId { get; private set; } = terminalId;

    public virtual Property Property { get; set; } = default!;
    public virtual Terminal Terminal { get; set; } = default!;
    
    public Session Update(DateTime? startDate, DateTime? endDate)
    {
        if (startDate is not null && StartDate != startDate)
            StartDate = startDate.Value;

        if (endDate is not null && EndDate != endDate)
            EndDate = endDate.Value;
        return this;
    }
}