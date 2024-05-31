namespace Travaloud.Domain.Stock;

public abstract class SupplierProduct(
        DefaultIdType supplierId, 
        DefaultIdType productId, 
        decimal? price, 
        decimal? salePrice,
        decimal? upfrontDiscount, 
        string? supplierCode)
    : AuditableEntity, IAggregateRoot
{
    private DefaultIdType SupplierId { get; set; } = supplierId;
    private DefaultIdType ProductId { get; set; } = productId;
    private decimal? Price { get; set; } = price;
    private decimal? SalePrice { get; set; } = salePrice;
    private decimal? UpfrontDiscount { get; set; } = upfrontDiscount;
    private string? SupplierCode { get; set; } = supplierCode;

    public virtual Product Product { get; set; } = default!;

    public SupplierProduct Update(decimal? price, decimal? salePrice, decimal? upfrontDiscount, string? supplierCode)
    {
        if (price is not null && Price != price)
            Price = price;

        if (salePrice is not null && SalePrice != salePrice)
            SalePrice = salePrice;

        if (upfrontDiscount is not null && UpfrontDiscount != upfrontDiscount)
            UpfrontDiscount = upfrontDiscount;

        if (supplierCode is not null && SupplierCode != supplierCode)
            SupplierCode = supplierCode;

        return this;
    }
}