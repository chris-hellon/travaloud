namespace Travaloud.Domain.Catalog.Tours;

public class TourDate : AuditableEntity, IAggregateRoot
{
    public DateTime StartDate { get; set; }
    public DefaultIdType TourId { get; set; }
    public DateTime EndDate { get; set; }
    public int AvailableSpaces { get; set; }
    public decimal? PriceOverride { get; set; }
    public DefaultIdType? TourPriceId { get; set; }
    public int ConcurrencyVersion { get; set; }
    public virtual TourPrice TourPrice { get; private set; } = default!;

    public TourDate()
    {

    }

    public TourDate(DateTime startDate, DateTime endDate, int availableSpaces, decimal? priceOverride = null, DefaultIdType? tourId = null, DefaultIdType? tourPriceId = null, TourPrice? tourPrice = null)
    {
        StartDate = startDate;
        EndDate = endDate;
        AvailableSpaces = availableSpaces;
        PriceOverride = priceOverride;
        TourPriceId = tourPriceId;

        if (tourPrice != null)
        {
            TourPrice = tourPrice;
        }

        if (tourId != null)
        {
            TourId = tourId.Value;
        }
    }

    public TourDate Update(DateTime? startDate, DateTime? endDate, int? availableSpaces, decimal? priceOverride, DefaultIdType? tourId, DefaultIdType? tourPriceId = null)
    {
        if (startDate is not null && !StartDate.Equals(startDate)) StartDate = startDate.Value;
        if (endDate is not null && !EndDate.Equals(endDate)) EndDate = endDate.Value;
        if (availableSpaces is not null && !AvailableSpaces.Equals(availableSpaces)) AvailableSpaces = availableSpaces.Value;
        if (tourId.HasValue && tourId.Value != DefaultIdType.Empty && !TourId.Equals(tourId.Value)) TourId = tourId.Value;
        if (priceOverride is not null && !PriceOverride.Equals(priceOverride)) PriceOverride = priceOverride.Value;
        if (tourPriceId.HasValue && tourPriceId.Value != DefaultIdType.Empty && !TourPriceId.Equals(tourPriceId.Value)) TourPriceId = tourPriceId.Value;

        return this;
    }
}