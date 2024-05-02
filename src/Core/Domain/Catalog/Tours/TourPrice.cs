namespace Travaloud.Domain.Catalog.Tours;

public class TourPrice : AuditableEntity, IAggregateRoot
{
    public decimal Price { get; set; }
    public DefaultIdType TourId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? MonthFrom { get; set; }
    public string? MonthTo { get; set; }
    public decimal? DayDuration { get; set; }
    public decimal? NightDuration { get; set; }
    public decimal? HourDuration { get; set; }
    public decimal? ComissionAmount { get; private set; }
    public bool? PublishToWebsite { get; private set; }

    public TourPrice()
    {

    }

    public TourPrice(decimal price,
        string title,
        string? description,
        string? monthFrom,
        string? monthTo,
        decimal? dayDuration,
        decimal? nightDuration,
        decimal? hourDuration,
        decimal? comissionAmount,
        bool? publishToWebsite,
        DefaultIdType? tourId = null)
    {
        Price = price;
        Title = title;
        MonthFrom = monthFrom;
        MonthTo = monthTo;
        Description = description;
        DayDuration = dayDuration;
        NightDuration = nightDuration;
        HourDuration = hourDuration;
        ComissionAmount = comissionAmount;
        PublishToWebsite = publishToWebsite;
        
        if (tourId != null)
        {
            TourId = tourId.Value;
        }
    }

    public TourPrice Update(decimal? price, string? title, string? description, string? monthFrom, string? monthTo, decimal? dayDuration, decimal? nightDuration, decimal? hourDuration, DefaultIdType? tourId,  decimal? comissionAmount, bool? publishToWebsite)
    {
        if (price.HasValue && Price != price) Price = price.Value;
        if (title is not null && Title?.Equals(title) is not true) Title = title;
        if (description is not null && Description?.Equals(description) is not true) Description = description;
        if (tourId.HasValue && tourId.Value != DefaultIdType.Empty && !TourId.Equals(tourId.Value)) TourId = tourId.Value;
        MonthFrom = monthFrom;
        MonthTo = monthTo;
        DayDuration = dayDuration;
        NightDuration = nightDuration;
        HourDuration = hourDuration;
        ComissionAmount = comissionAmount;
        PublishToWebsite = publishToWebsite;
        
        return this;
    }
}