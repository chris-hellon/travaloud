using Travaloud.Domain.Identity;

namespace Travaloud.Domain.Stock;

public abstract class OrderSupplierNote(
    DefaultIdType orderId, 
    DefaultIdType supplierId, 
    string notes) : AuditableEntity, IAggregateRoot
{
    private DefaultIdType OrderId { get; set; } = orderId;
    private DefaultIdType SupplierId { get; set; } = supplierId;
    private string Notes { get; set; } = notes;

    public virtual ApplicationUser Supplier { get; set; } = default!;

    public OrderSupplierNote Update(string notes)
    {
        if (!string.Equals(Notes, notes))
            Notes = notes;

        return this;
    }
}