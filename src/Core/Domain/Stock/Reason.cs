namespace Travaloud.Domain.Stock;

public abstract class Reason(
    string description,
    bool isWaste,
    bool isReturn) : BaseEntity, IAggregateRoot
{
    private string Description { get; set; } = description;
    private bool IsWaste { get; set; } = isWaste;
    private bool IsReturn { get; set; } = isReturn;

    public Reason Update(string? description, bool? isWaste, bool? isReturn)
    {
        if (description is not null && Description != description)
            Description = description;

        if (isWaste is not null && IsWaste != isWaste.Value)
            IsWaste = isWaste.Value;

        if (isReturn is not null && IsReturn != isReturn.Value)
            IsReturn = isReturn.Value;

        return this;
    }
}
