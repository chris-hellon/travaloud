namespace Travaloud.Domain.PointOfSale;

public abstract class VoidReason(
    string name) 
    : BaseEntity, IAggregateRoot
{
    [MaxLength(200)] 
    public string Name { get; private set; } = name;
    
    public VoidReason Update(string? name)
    {
        if (name is not null && Name != name)
            Name = name;
        
        return this;
    }
}