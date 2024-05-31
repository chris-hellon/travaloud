namespace Travaloud.Domain.Common.Enums;

public enum OrderStatus
{
    [Display(Name = "Order Created")]
    OrderCreated,
    [Display(Name = "Supplier Emailed")]
    SupplierEmailed,
    [Display(Name = "Order Delivered")]
    OrderDelivered,
    [Display(Name = "Order Submitted")]
    OrderSubmitted,
    [Display(Name = "Order Reviewed")]
    OrderReviewed,
    [Display(Name = "Order Reported")]
    OrderReported
}