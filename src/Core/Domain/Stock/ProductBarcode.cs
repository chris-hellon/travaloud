namespace Travaloud.Domain.Stock;

public abstract class ProductBarcode(DefaultIdType productId, DefaultIdType barcodeId) : BaseEntity, IAggregateRoot
{
    private DefaultIdType ProductId { get; set; } = productId;
    private DefaultIdType BarcodeId { get; set; } = barcodeId;
    
    public virtual Barcode Barcode { get; set; } = default!;
}