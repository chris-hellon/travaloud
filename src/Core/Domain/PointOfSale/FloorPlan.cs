using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Domain.PointOfSale;

public abstract class FloorPlan(
        DefaultIdType propertyId, 
        string title, 
        string? description, 
        string? imageUrl)
    : AuditableEntity, IAggregateRoot
{
    public DefaultIdType PropertyId { get; private set; } = propertyId;
    
    [MaxLength(300)]
    public string Title { get; private set; } = title;
    
    public string? Description { get; private set; } = description;
    
    [MaxLength(400)]
    public string? ImageUrl { get; private set; } = imageUrl;

    public virtual Property Property { get; set; } = default!;
    public virtual IEnumerable<FloorPlanTable>? Tables { get; set; } = default!;
    
    public FloorPlan Update(string? title, string? description, string? imageUrl)
    {
        if (title is not null && Title != title)
            Title = title;

        if (description is not null && Description != description)
            Description = description;

        if (imageUrl is not null && ImageUrl != imageUrl)
            ImageUrl = imageUrl;

        return this;
    }
}