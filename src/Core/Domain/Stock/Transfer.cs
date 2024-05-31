using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Domain.Stock;

public abstract class Transfer(
        DefaultIdType productId, 
        bool received, 
        DateTime? dateRequested, 
        DateTime? dateReceived, 
        DefaultIdType propertyFromId,
        DefaultIdType propertyToId, 
        DefaultIdType sentById, 
        DefaultIdType? receivedById, 
        int quantity, 
        decimal? split)
    : AuditableEntity, IAggregateRoot
{
    private DefaultIdType ProductId { get; set; } = productId;
    private bool Received { get; set; } = received;
    private DateTime? DateRequested { get; set; } = dateRequested;
    private DateTime? DateReceived { get; set; } = dateReceived;
    private DefaultIdType PropertyFromId { get; set; } = propertyFromId;
    private DefaultIdType PropertyToId { get; set; } = propertyToId;
    private DefaultIdType SentById { get; set; } = sentById;
    private DefaultIdType? ReceivedById { get; set; } = receivedById;
    private int Quantity { get; set; } = quantity;
    private decimal? Split { get; set; } = split;

    public virtual Product Product { get; set; } = default!;
    public virtual Property PropertyFrom { get; set; } = default!;
    public virtual Property PropertyTo { get; set; } = default!;
    
    public Transfer Update(
        DefaultIdType? productId, 
        bool? received, 
        DateTime? dateRequested, 
        DateTime? dateReceived, 
        DefaultIdType? propertyFromId,
        DefaultIdType? propertyToId, 
        DefaultIdType? sentById, 
        DefaultIdType? receivedById, 
        int? quantity, 
        decimal? split)
    {
        if (productId is not null && productId != ProductId)
            ProductId = productId.Value;

        if (received is not null && received != Received)
            Received = received.Value;

        if (dateRequested is not null && dateRequested != DateRequested)
            DateRequested = dateRequested.Value;

        if (dateReceived is not null && dateReceived != DateReceived)
            DateReceived = dateReceived.Value;

        if (propertyFromId is not null && propertyFromId != PropertyFromId)
            PropertyFromId = propertyFromId.Value;

        if (propertyToId is not null && propertyToId != PropertyToId)
            PropertyToId = propertyToId.Value;

        if (sentById is not null && sentById != SentById)
            SentById = sentById.Value;

        if (receivedById is not null && receivedById != ReceivedById)
            ReceivedById = receivedById.Value;

        if (quantity is not null && quantity != Quantity)
            Quantity = quantity.Value;

        if (split is not null && split != Split)
            Split = split.Value;

        return this;
    }
}