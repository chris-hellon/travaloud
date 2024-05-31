using Travaloud.Domain.Catalog.Properties;
using Travaloud.Domain.Identity;

namespace Travaloud.Domain.Stock;

public abstract class Report(
        DefaultIdType propertyId, 
        string notes, 
        DateTime? submitDate, 
        DefaultIdType? submittedBy, 
        bool? isFood,
        bool? isConsumable)
    : AuditableEntity, IAggregateRoot
{
    private DefaultIdType PropertyId { get; set; } = propertyId;
    private string Notes { get; set; } = notes;
    private DateTime? SubmitDate { get; set; } = submitDate;
    private DefaultIdType? SubmittedBy { get; set; } = submittedBy;
    private bool? IsFood { get; set; } = isFood;
    private bool? IsConsumable { get; set; } = isConsumable;

    public virtual ApplicationUser SubmittedById { get; set; } = default!;
    public virtual Property Property { get; set; } = default!;
    public virtual IEnumerable<ReportProduct> Products { get; set; } = default!;

    public Report Update(string? notes, DateTime? submitDate, DefaultIdType? submittedBy, bool? isFood, bool? isConsumable)
    {
        if (notes is not null && Notes != notes)
            Notes = notes;

        if (submitDate is not null && SubmitDate != submitDate)
            SubmitDate = submitDate;
        
        if (submittedBy is not null && SubmittedBy != submittedBy)
            SubmittedBy = submittedBy;

        if (isFood is not null && IsFood != isFood)
            IsFood = isFood;

        if (isConsumable is not null && IsConsumable != isConsumable)
            IsConsumable = isConsumable;

        return this;
    }
}