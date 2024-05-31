namespace Travaloud.Domain.Stock;

public abstract class ReportProduct(
        DefaultIdType reportId, 
        DefaultIdType productId, 
        decimal sealedCount, 
        decimal openCount, 
        decimal sales,
        decimal waste, 
        decimal? computedCount, 
        decimal percentageVariance, 
        decimal monetaryVariance,
        decimal? absoluteVariance)
    : AuditableEntity, IAggregateRoot
{
    private DefaultIdType ReportId { get; set; } = reportId;
    private DefaultIdType ProductId { get; set; } = productId;
    private decimal SealedCount { get; set; } = sealedCount;
    private decimal OpenCount { get; set; } = openCount;
    private decimal Sales { get; set; } = sales;
    private decimal Waste { get; set; } = waste;
    private decimal? ComputedCount { get; set; } = computedCount;
    private decimal PercentageVariance { get; set; } = percentageVariance;
    private decimal MonetaryVariance { get; set; } = monetaryVariance;
    private decimal? AbsoluteVariance { get; set; } = absoluteVariance;

    public virtual Report Report { get; set; } = default!;
    public virtual Product Product { get; set; } = default!;

    public ReportProduct Update(decimal sealedCount, decimal openCount, decimal sales, decimal waste, decimal? computedCount, decimal percentageVariance, decimal monetaryVariance, decimal? absoluteVariance)
    {
        if (sealedCount != SealedCount)
            SealedCount = sealedCount;

        if (openCount != OpenCount)
            OpenCount = openCount;

        if (sales != Sales)
            Sales = sales;

        if (waste != Waste)
            Waste = waste;

        if (computedCount != ComputedCount)
            ComputedCount = computedCount;

        if (percentageVariance != PercentageVariance)
            PercentageVariance = percentageVariance;

        if (monetaryVariance != MonetaryVariance)
            MonetaryVariance = monetaryVariance;

        if (absoluteVariance != AbsoluteVariance)
            AbsoluteVariance = absoluteVariance;

        return this;
    }
}