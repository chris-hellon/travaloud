namespace Travaloud.Application.Catalog.Tours.Dto;

public class TourPriceDto 
{
    public DefaultIdType Id { get; set; }
    public decimal Price { get; set; }
    public DefaultIdType TourId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? MonthFrom { get; set; }
    public string? MonthTo { get; set; }
    public decimal? DayDuration { get; set; }
    public decimal? NightDuration { get; set; }
    public decimal? HourDuration { get; set; }
    public decimal? ComissionAmount { get; set; }
}