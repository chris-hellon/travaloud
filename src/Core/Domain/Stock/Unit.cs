namespace Travaloud.Domain.Stock;

public abstract class Unit(string description, string unitMetric, string isWeight) : BaseEntity, IAggregateRoot
{
    private string Description { get; set; } = description;
    private string UnitMetric { get; set; } = unitMetric;
    private string IsWeight { get; set; } = isWeight;

    public Unit Update(string? description, string? unitMetric, string? isWeight)
    {
        if (description is not null && Description != description)
            Description = description;

        if (unitMetric is not null && UnitMetric != unitMetric)
            UnitMetric = unitMetric;

        if (isWeight is not null && IsWeight != isWeight)
            IsWeight = isWeight;

        return this;
    }
}