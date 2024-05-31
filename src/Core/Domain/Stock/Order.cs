using Travaloud.Domain.Catalog.Properties;
using Travaloud.Domain.Common.Enums;

namespace Travaloud.Domain.Stock;

public abstract class Order(
        DefaultIdType propertyId, 
        DateTime? placedDate, 
        DateTime? deliveredDate, 
        DateTime? reviewedDate, 
        DefaultIdType? placedBy,
        DefaultIdType? deliveredBy, 
        DefaultIdType? reviewedBy, 
        DefaultIdType? underReviewUserId, 
        OrderStatus orderStatus, 
        OrderType orderType)
    : AuditableEntity, IAggregateRoot
{
    private DefaultIdType PropertyId { get; set; } = propertyId;

    private DateTime? PlacedDate { get; set; } = placedDate;
    private DateTime? DeliveredDate { get; set; } = deliveredDate;
    private DateTime? ReviewedDate { get; set; } = reviewedDate;

    private DefaultIdType? PlacedBy { get; set; } = placedBy;
    private DefaultIdType? DeliveredBy { get; set; } = deliveredBy;
    private DefaultIdType? ReviewedBy { get; set; } = reviewedBy;
    private DefaultIdType? UnderReviewUserId { get; set; } = underReviewUserId;

    private OrderStatus OrderStatus { get; set; } = orderStatus;
    private OrderType OrderType { get; set; } = orderType;

    public virtual Property Property { get; set; } = default!;
    public virtual IEnumerable<OrderProduct> OrderProducts { get; set; } = default!;
    public virtual IEnumerable<OrderSupplierNote>? SupplierNotes { get; set; } = default!;

    public Order Update(
        DateTime? placedDate,
        DateTime? deliveredDate,
        DateTime? reviewedDate,
        DefaultIdType? placedBy,
        DefaultIdType? deliveredBy,
        DefaultIdType? reviewedBy,
        DefaultIdType? underReviewUserId,
        OrderStatus? orderStatus,
        OrderType? orderType)
    {
        if (placedDate.HasValue && PlacedDate != placedDate)
            PlacedDate = placedDate;

        if (deliveredDate.HasValue && DeliveredDate != deliveredDate)
            DeliveredDate = deliveredDate;

        if (reviewedDate.HasValue && ReviewedDate != reviewedDate)
            ReviewedDate = reviewedDate;

        if (placedBy.HasValue && PlacedBy != placedBy)
            PlacedBy = placedBy;

        if (deliveredBy.HasValue && DeliveredBy != deliveredBy)
            DeliveredBy = deliveredBy;

        if (reviewedBy.HasValue && ReviewedBy != reviewedBy)
            ReviewedBy = reviewedBy;

        if (underReviewUserId.HasValue && UnderReviewUserId != underReviewUserId)
            UnderReviewUserId = underReviewUserId;

        if (orderStatus is not null && OrderStatus != orderStatus)
            OrderStatus = orderStatus.Value;

        if (orderType is not null && OrderType != orderType)
            OrderType = orderType.Value;
        
        return this;
    }
}