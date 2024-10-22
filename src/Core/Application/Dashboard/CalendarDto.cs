namespace Travaloud.Application.Dashboard;

public class CalendarDto
{
    public string Tour { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int GuestCount { get; set; }
    public DefaultIdType TourId { get; set; }
    // public DefaultIdType TourDateId { get; set; }
}