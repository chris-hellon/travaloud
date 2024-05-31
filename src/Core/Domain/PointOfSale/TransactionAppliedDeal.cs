namespace Travaloud.Domain.PointOfSale;

public abstract class TransactionAppliedDeal(
        string dealName, 
        bool applied, 
        decimal amount,
        DefaultIdType dealId, 
        DefaultIdType transactionId)
    : BaseEntity, IAggregateRoot
{
    [MaxLength(450)] 
    public string DealName { get; private set; } = dealName;
    
    public bool Applied { get; private set; } = applied;
    
    [Precision(19,4)] 
    public decimal Amount { get; private set; } = amount;
    
    public DefaultIdType DealId { get; private set; } = dealId;
    
    public DefaultIdType TransactionId { get; private set; } = transactionId;

    public virtual Deal Deal { get; set; } = default!;
    
    public TransactionAppliedDeal Update(string? dealName, bool? applied, decimal? amount, DefaultIdType? dealId, DefaultIdType? transactionId)
    {
        if (dealName is not null && DealName != dealName)
            DealName = dealName;

        if (applied is not null && Applied != applied)
            Applied = applied.Value;

        if (amount is not null && Amount != amount)
            Amount = amount.Value;
        
        if (dealId is not null && DealId != dealId)
            DealId = dealId.Value;

        if (transactionId is not null && TransactionId != transactionId)
            TransactionId = transactionId.Value;

        return this;
    }
}