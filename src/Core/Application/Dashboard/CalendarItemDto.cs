namespace Travaloud.Application.Dashboard;

public class CalendarItemDto
{
    public string GuestId { get; set; }
    public int Reference { get; set; }
    public string GuestName { get; set; }
    public bool WaiverSigned { get; set; }
    public string PickupLocation { get; set; }
}