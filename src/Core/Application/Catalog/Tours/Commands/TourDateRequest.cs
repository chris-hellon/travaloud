namespace Travaloud.Application.Catalog.Tours.Commands;

public class TourDateRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public DateTime? StartDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public DefaultIdType TourId { get; set; }
    public DateTime? EndDate { get; set; }
    public TimeSpan? EndTime { get; set; }
    public int? AvailableSpaces { get; set; }
    public decimal? PriceOverride { get; set; }
    public DefaultIdType? TourPriceId { get; set; }
    public bool IsCreate { get; set; }
    public bool Repeats { get; set; }
    public string? RepeatsCondition { get; set; }
    public int? RepeatsDuration { get; set; }
    public int? RepeatsCount { get; set; }
}