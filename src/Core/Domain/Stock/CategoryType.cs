namespace Travaloud.Domain.Stock;

public abstract class CategoryType(string name) 
    : AuditableEntity, IAggregateRoot
{
    private string Name { get; set; } = name;
    
    public CategoryType Update(string? name)
    {
        if (name is not null && Name != name)
            Name = name;

        return this;
    }
}