using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Domain.Identity;

namespace Travaloud.Domain.PointOfSale;

public abstract class Transaction(
        int status,
        int sequenceNumber,
        int transactionNumber,
        DateTime transactionStartDateTime,
        DateTime transactionCloseDateTime,
        int covers,
        bool voided,
        decimal? totalValue,
        bool? isWaste,
        DefaultIdType? bookingId,
        DefaultIdType sessionId,
        DefaultIdType operatorId,
        DefaultIdType floorPlanTableId)
    : BaseEntity, IAggregateRoot
{
    public int Status { get; private set; } = status;
    
    public int SequenceNumber { get; private set; } = sequenceNumber;
    
    public int TransactionNumber { get; private set; } = transactionNumber;
    
    public DateTime TransactionStartDateTime { get; private set; } = transactionStartDateTime;
    
    public DateTime TransactionCloseDateTime { get; private set; } = transactionCloseDateTime;
    
    public int Covers { get; private set; } = covers;
    
    public bool Voided { get; private set; } = voided;
    
    [Precision(19, 4)] 
    public decimal? TotalValue { get; private set; } = totalValue;
    
    public bool? IsWaste { get; private set; } = isWaste;
    
    public DefaultIdType? BookingId { get; private set; } = bookingId;
    
    public DefaultIdType SessionId { get; private set; } = sessionId;
    
    public DefaultIdType OperatorId { get; private set; } = operatorId;
    
    public DefaultIdType FloorPlanTableId { get; private set; } = floorPlanTableId;
    
    public virtual IEnumerable<TransactionLineItem>? TransactionLineItems { get; set; } = default!;
    public virtual IEnumerable<TransactionAppliedDeal>? TransactionAppliedDeals { get; set; } = default!;
    public virtual IEnumerable<TransactionPayment>? TransactionPayments { get; set; } = default!;
    public virtual Booking? Booking { get; set; } = default!;
    public virtual Session Session { get; set; } = default!;
    public virtual ApplicationUser Operator { get; set; } = default!;
    public virtual FloorPlanTable FloorPlanTable { get; set; } = default!;
    
    public Transaction Update(int? status, int? sequenceNumber, int? transactionNumber,
        DateTime? transactionStartDateTime, DateTime? transactionCloseDateTime, int? covers, bool? voided,
        decimal? totalValue, bool? isWaste, DefaultIdType? bookingId,
        DefaultIdType? sessionId,
        DefaultIdType? operatorId,
        DefaultIdType? floorPlanTableId)
    {
        if (sessionId is not null && SessionId != sessionId)
            SessionId = sessionId.Value;

        if (operatorId is not null && OperatorId != operatorId)
            OperatorId = operatorId.Value;

        if (floorPlanTableId is not null && FloorPlanTableId != floorPlanTableId)
            FloorPlanTableId = floorPlanTableId.Value;

        if (status is not null && Status != status)
            Status = status.Value;

        if (sequenceNumber is not null && SequenceNumber != sequenceNumber)
            SequenceNumber = sequenceNumber.Value;

        if (transactionNumber is not null && TransactionNumber != transactionNumber)
            TransactionNumber = transactionNumber.Value;

        if (transactionStartDateTime is not null && TransactionStartDateTime != transactionStartDateTime)
            TransactionStartDateTime = transactionStartDateTime.Value;

        if (transactionCloseDateTime is not null && TransactionCloseDateTime != transactionCloseDateTime)
            TransactionCloseDateTime = transactionCloseDateTime.Value;

        if (covers is not null && Covers != covers)
            Covers = covers.Value;

        if (voided is not null && Voided != voided)
            Voided = voided.Value;

        if (totalValue is not null && TotalValue != totalValue)
            TotalValue = totalValue;

        if (isWaste is not null && IsWaste != isWaste)
            IsWaste = isWaste;
        
        if (bookingId is not null && BookingId != bookingId)
            BookingId = bookingId;

        return this;
    }
}