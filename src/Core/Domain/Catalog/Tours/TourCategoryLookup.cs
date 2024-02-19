namespace Travaloud.Domain.Catalog.Tours;

public class TourCategoryLookup : AuditableEntity, IAggregateRoot
{
    public DefaultIdType? TourId { get; private set; }
    public DefaultIdType TourCategoryId { get; set; }
    public DefaultIdType? ParentTourCategoryId { get; private set; }

    public virtual Tour Tour { get; private set; } = default!;
    public virtual TourCategory TourCategory { get; private set; } = default!;
    public virtual TourCategory ParentTourCategory { get; private set; } = default!;

    public TourCategoryLookup(DefaultIdType tourCategoryId, DefaultIdType? parentTourCategoryId, DefaultIdType? tourId = null)
    {
        TourCategoryId = tourCategoryId;
        ParentTourCategoryId = parentTourCategoryId;

        if (tourId != null)
        {
            TourId = tourId.Value;
        }
    }

    public TourCategoryLookup Update(DefaultIdType? tourId, DefaultIdType tourCategoryId, DefaultIdType? parentTourCategoryId)
    {
        if (tourId.HasValue && tourId.Value != DefaultIdType.Empty && !TourId.Equals(tourId.Value)) TourId = tourId.Value;
        if (tourCategoryId != DefaultIdType.Empty && !TourCategoryId.Equals(tourCategoryId)) TourId = tourCategoryId;
        if (parentTourCategoryId.HasValue && parentTourCategoryId.Value != DefaultIdType.Empty && !ParentTourCategoryId.Equals(parentTourCategoryId.Value)) ParentTourCategoryId = parentTourCategoryId.Value;
        return this;
    }
}