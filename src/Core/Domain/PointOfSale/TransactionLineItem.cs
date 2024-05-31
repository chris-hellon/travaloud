using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Domain.PointOfSale;

public abstract class TransactionLineItem(
        decimal? taxRate, 
        DateTime dateTimeAdded, 
        string description, 
        int quantity,
        decimal itemPrice, 
        decimal totalPrice,
        DefaultIdType? menuItemId, 
        DefaultIdType? modifierId, 
        DefaultIdType? bookingId, 
        DefaultIdType? parentTransactionLineItemId,
        DefaultIdType transactionId)
    : BaseEntity, IAggregateRoot
{
    [Precision(5, 2)] 
    public decimal? TaxRate { get; private set; } = taxRate;
    
    public DateTime DateTimeAdded { get; private set; } = dateTimeAdded;
    
    [MaxLength(450)] 
    public string Description { get; private set; } = description;
    
    public int Quantity { get; private set; } = quantity;
    
    [Precision(19,4)] 
    public decimal ItemPrice { get; private set; } = itemPrice;
    
    [Precision(19,4)] 
    public decimal TotalPrice { get; private set; } = totalPrice;

    public DefaultIdType? MenuItemId { get; set; } = menuItemId;
    public DefaultIdType? ModifierId { get; set; } = modifierId;
    public DefaultIdType? BookingId { get; set; } = bookingId;
    public DefaultIdType? ParentTransactionLineItemId { get; set; } = parentTransactionLineItemId;
    public DefaultIdType TransactionId { get; set; } = transactionId;

    public virtual TransactionLineItem? ParentTransactionLineItem { get; set; }
    public virtual List<TransactionLineItem> ChildTransactionLineItems { get; set; } = new();
    public virtual MenuItem? MenuItem { get; set; } = default!;
    public virtual Modifier? Modifier { get; set; } = default!;
    public virtual Booking? Booking { get; set; } = default!;

    public TransactionLineItem Update(decimal? taxRate,
        DateTime? dateTimeAdded,
        string? description,
        int? quantity,
        decimal? itemPrice,
        decimal? totalPrice,
        DefaultIdType? parentTransactionLineItemId,
        DefaultIdType? menuItemId,
        DefaultIdType? modifierId,
        DefaultIdType? bookingId,
        DefaultIdType? transactionId)
    {
        if (taxRate is not null && TaxRate != taxRate)
            TaxRate = taxRate.Value;

        if (dateTimeAdded is not null && DateTimeAdded != dateTimeAdded)
            DateTimeAdded = dateTimeAdded.Value;

        if (description is not null && Description != description)
            Description = description;

        if (quantity is not null && Quantity != quantity)
            Quantity = quantity.Value;

        if (itemPrice is not null && ItemPrice != itemPrice)
            ItemPrice = itemPrice.Value;

        if (totalPrice is not null && TotalPrice != totalPrice)
            TotalPrice = totalPrice.Value;
        
        if (parentTransactionLineItemId is not null && ParentTransactionLineItemId != parentTransactionLineItemId)
            ParentTransactionLineItemId = parentTransactionLineItemId.Value;
        
        if (menuItemId is not null && MenuItemId != menuItemId)
            MenuItemId = menuItemId.Value;

        if (modifierId is not null && ModifierId != modifierId)
            ModifierId = modifierId.Value;

        if (bookingId is not null && BookingId != bookingId)
            BookingId = bookingId.Value;

        if (transactionId is not null && TransactionId != transactionId)
            TransactionId = transactionId.Value;

        return this;
    }
}