using System.ComponentModel.DataAnnotations;

namespace Travaloud.Application.Catalog.Tours.Commands;

public class TourPriceRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public decimal? Price { get; set; }
    public DefaultIdType TourId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? MonthFrom { get; set; }
    public string? MonthTo { get; set; }

    [Display(Name = "Day Duration")]
    [RequiredIfNull("DayDuration", "NightDuration", "HourDuration", ErrorMessage = "A Day, Night or Hour Duration is required")]
    public decimal? DayDuration { get; set; }

    [Display(Name = "Night Duration")]
    public decimal? NightDuration { get; set; }

    [Display(Name = "Hour Duration")]
    public decimal? HourDuration { get; set; }

    public bool IsCreate { get; set; }
    public decimal? ComissionAmount { get; set; }
    public IList<TourDateRequest>? Dates { get; set; }
}