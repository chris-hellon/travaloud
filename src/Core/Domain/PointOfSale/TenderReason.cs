namespace Travaloud.Domain.PointOfSale;

public abstract class TenderReason(
        string name) 
    : BaseEntity, IAggregateRoot
{
    [MaxLength(300)] 
    public string Name { get; private set; } = name;

    public TenderReason Update(string? name)
    {
        if (name is not null && Name != name)
            Name = name;
        
        return this;
    }
}