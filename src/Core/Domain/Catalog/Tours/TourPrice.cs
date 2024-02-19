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

    public TourPrice()
    {

    }

    public TourPrice(decimal price, string title, string? description, string? monthFrom, string? monthTo, decimal? dayDuration, decimal? nightDuration, decimal? hourDuration, DefaultIdType? tourId = null)
    {
        Price = price;
        Title = title;
        MonthFrom = monthFrom;
        MonthTo = monthTo;
        Description = description;
        DayDuration = dayDuration;
        NightDuration = nightDuration;
        HourDuration = hourDuration;

        if (tourId != null)
        {
            TourId = tourId.Value;
        }
    }

    public TourPrice Update(decimal? price, string? title, string? description, string? monthFrom, string? monthTo, decimal? dayDuration, decimal? nightDuration, decimal? hourDuration, DefaultIdType? tourId)
    {
        if (price.HasValue && Price != price) Price = price.Value;
        if (title is not null && Title?.Equals(title) is not true) Title = title;
        if (description is not null && Description?.Equals(description) is not true) Description = description;
        if (tourId.HasValue && tourId.Value != DefaultIdType.Empty && !TourId.Equals(tourId.Value)) TourId = tourId.Value;
        if (monthFrom != null && MonthFrom != monthFrom) MonthFrom = monthFrom;
        if (monthTo != null && MonthTo != monthTo) MonthTo = monthTo;
        if (dayDuration.HasValue && dayDuration.Value != 0 && !DayDuration.Equals(dayDuration.Value)) DayDuration = dayDuration.Value;
        if (nightDuration.HasValue && nightDuration.Value != 0 && !NightDuration.Equals(nightDuration.Value)) NightDuration = nightDuration.Value;
        if (hourDuration.HasValue && hourDuration.Value != 0 && !HourDuration.Equals(hourDuration.Value)) HourDuration = hourDuration.Value;

        return this;
    }
}