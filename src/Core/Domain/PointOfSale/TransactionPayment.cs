namespace Travaloud.Domain.PointOfSale;

public abstract class TransactionPayment(
        string tenderMediaName, 
        decimal? tendered, 
        decimal? change, 
        DefaultIdType tenderMediaId,
        DefaultIdType? tenderReasonId, 
        DefaultIdType transactionId)
    : BaseEntity, IAggregateRoot
{
    [MaxLength(450)] 
    public string TenderMediaName { get; private set; } = tenderMediaName;
    
    [Precision(19,4)] 
    public decimal? Tendered { get; private set; } = tendered;
    
    [Precision(19,4)] 
    public decimal? Change { get; private set; } = change;
    
    public DefaultIdType TenderMediaId { get; private set; } = tenderMediaId;
    
    public DefaultIdType? TenderReasonId { get; private set; } = tenderReasonId;
    
    public DefaultIdType TransactionId { get; private set; } = transactionId;

    public virtual Transaction Transaction { get; set; } = default!;
    public virtual TenderMedia TenderMedia { get; set; } = default!;
    public virtual TenderReason TenderReason { get; set; } = default!;

    public TransactionPayment Update(string? tenderMediaName, decimal? tendered, decimal? change, DefaultIdType? tenderMediaId, DefaultIdType? tenderReasonId, DefaultIdType? transactionId)
    {
        if (tenderMediaName is not null && TenderMediaName != tenderMediaName)
            TenderMediaName = tenderMediaName;

        if (tendered is not null && Tendered != tendered)
            Tendered = tendered;

        if (change is not null && Change != change)
            Change = change;

        if (tenderMediaId is not null && TenderMediaId != tenderMediaId)
            TenderMediaId = tenderMediaId.Value;

        if (tenderReasonId is not null && TenderReasonId != tenderReasonId)
            TenderReasonId = tenderReasonId.Value;

        if (transactionId is not null && TransactionId != transactionId)
            TransactionId = transactionId.Value;

        return this;
    }

}