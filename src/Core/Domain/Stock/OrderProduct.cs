using Travaloud.Domain.Identity;

namespace Travaloud.Domain.Stock;

public abstract class OrderProduct(
    DefaultIdType orderId,
    DefaultIdType productId,
    int? quantityOrdered,
    int? quantityDelivered,
    DefaultIdType? deliveredBy,
    DefaultIdType? deliveredSupplierId, DateTime? deliveredDate, decimal? deliveredPrice, decimal? orderedPrice,
    decimal? orderedInvoicePrice, DefaultIdType? supplierId, DefaultIdType? supplierOfferId, bool? isEmailed, bool isReward, bool? isAppCreated) :
    BaseEntity, IAggregateRoot
{
    private DefaultIdType OrderId { get; set; } = orderId;
    private DefaultIdType ProductId { get; set; } = productId;
    private int? QuantityOrdered { get; set; } = quantityOrdered;
    private int? QuantityDelivered { get; set; } = quantityDelivered;
    private DefaultIdType? DeliveredBy { get; set; } = deliveredBy;
    private DefaultIdType? DeliveredSupplierId { get; set; } = deliveredSupplierId;
    private DateTime? DeliveredDate { get; set; } = deliveredDate;
    private decimal? DeliveredPrice { get; set; } = deliveredPrice;
    private decimal? OrderedPrice { get; set; } = orderedPrice;
    private decimal? OrderedInvoicePrice { get; set; } = orderedInvoicePrice;
    private DefaultIdType? SupplierId { get; set; } = supplierId;
    private DefaultIdType? SupplierOfferId { get; set; } = supplierOfferId;
    private bool? IsEmailed { get; set; } = isEmailed;
    private bool IsReward { get; set; } = isReward;
    private bool? IsAppCreated { get; set; } = isAppCreated;
    
    public virtual Product Product { get; set; } = default!;
    public virtual ApplicationUser? Supplier { get; set; } = default!;
    public virtual ApplicationUser? DeliveredSupplier { get; set; } = default!;
    public virtual SupplierOffer? SupplierOffer { get; set; } = default!;

    public OrderProduct Update(
        int? quantityOrdered,
        int? quantityDelivered,
        DefaultIdType? deliveredBy,
        DefaultIdType? deliveredSupplierId,
        DateTime? deliveredDate,
        decimal? deliveredPrice,
        decimal? orderedPrice,
        decimal? orderedInvoicePrice,
        DefaultIdType? supplierId,
        DefaultIdType? supplierOfferId,
        bool? isEmailed,
        bool isReward,
        bool? isAppCreated)
    {
        if (quantityOrdered.HasValue && QuantityOrdered != quantityOrdered)
            QuantityOrdered = quantityOrdered;

        if (quantityDelivered.HasValue && QuantityDelivered != quantityDelivered)
            QuantityDelivered = quantityDelivered;

        if (deliveredBy.HasValue && DeliveredBy != deliveredBy)
            DeliveredBy = deliveredBy;

        if (deliveredSupplierId.HasValue && DeliveredSupplierId != deliveredSupplierId)
            DeliveredSupplierId = deliveredSupplierId;

        if (deliveredDate.HasValue && DeliveredDate != deliveredDate)
            DeliveredDate = deliveredDate;

        if (deliveredPrice.HasValue && DeliveredPrice != deliveredPrice)
            DeliveredPrice = deliveredPrice;

        if (orderedPrice.HasValue && OrderedPrice != orderedPrice)
            OrderedPrice = orderedPrice;

        if (orderedInvoicePrice.HasValue && OrderedInvoicePrice != orderedInvoicePrice)
            OrderedInvoicePrice = orderedInvoicePrice;

        if (supplierId.HasValue && SupplierId != supplierId)
            SupplierId = supplierId;

        if (supplierOfferId.HasValue && SupplierOfferId != supplierOfferId)
            SupplierOfferId = supplierOfferId;

        if (isEmailed.HasValue && IsEmailed != isEmailed)
            IsEmailed = isEmailed;

        if (IsReward != isReward)
            IsReward = isReward;

        if (isAppCreated.HasValue && IsAppCreated != isAppCreated)
            IsAppCreated = isAppCreated;
        
        return this;
    }
}