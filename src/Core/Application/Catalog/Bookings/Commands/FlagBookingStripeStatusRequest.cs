namespace Travaloud.Application.Catalog.Bookings.Commands;

public class FlagBookingStripeStatusRequest : IRequest
{
    public DefaultIdType BookingId { get; set; }
    public string SessionId { get; set; }

    public FlagBookingStripeStatusRequest(DefaultIdType bookingId, string sessionId)
    {
        BookingId = bookingId;
        SessionId = sessionId;
    }
}

internal class FlagBookingStripeStatusHandler : IRequestHandler<FlagBookingStripeStatusRequest>
{
    private readonly IDapperRepository _dapperRepository;

    public FlagBookingStripeStatusHandler(IDapperRepository dapperRepository)
    {
        _dapperRepository = dapperRepository;
    }

    public async Task Handle(FlagBookingStripeStatusRequest request, CancellationToken cancellationToken)
    {
        await _dapperRepository.ExecuteAsync(
            $"UPDATE [Catalog].Bookings SET StripeSessionId = '{request.SessionId}' WHERE Id = '{request.BookingId}'", cancellationToken: cancellationToken);
    }
}