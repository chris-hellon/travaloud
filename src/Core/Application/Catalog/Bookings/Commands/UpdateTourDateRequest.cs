namespace Travaloud.Application.Catalog.Bookings.Commands;

public class UpdateTourDateRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public int ConcurrencyVersion { get; set; }
}