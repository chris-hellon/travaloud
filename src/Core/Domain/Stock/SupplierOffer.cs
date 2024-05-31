using Travaloud.Domain.Identity;

namespace Travaloud.Domain.Stock;

public abstract class SupplierOffer(
        DefaultIdType supplierId,
        string? description, 
        DateTime? startDate, 
        DateTime? endDate)
    : AuditableEntity, IAggregateRoot
{
    private DefaultIdType SupplierId { get; set; } = supplierId;
    private string? Description { get; set; } = description;
    private DateTime? StartDate { get; set; } = startDate;
    private DateTime? EndDate { get; set; } = endDate;

    public virtual ApplicationUser Supplier { get; set; } = default!;

    public SupplierOffer Update(string? description, DateTime? startDate, DateTime? endDate)
    {
        if (description is not null && Description != description)
            Description = description;

        if (startDate is not null && StartDate != startDate)
            StartDate = startDate;

        if (endDate is not null && EndDate != endDate)
            EndDate = endDate;

        return this;
    }
}