using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Commands;

public class FlagBookingConfirmationEmailRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
}

internal class FlagBookingConfirmationEmailRequestHandler : IRequestHandler<FlagBookingConfirmationEmailRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Booking> _bookingRepository;
    private readonly IStringLocalizer<FlagBookingConfirmationEmailRequestHandler> _localizer;

    public FlagBookingConfirmationEmailRequestHandler(IRepositoryFactory<Booking> bookingRepository, IStringLocalizer<FlagBookingConfirmationEmailRequestHandler> localizer)
    {
        _bookingRepository = bookingRepository;
        _localizer = localizer;
    }

    public async Task<DefaultIdType> Handle(FlagBookingConfirmationEmailRequest request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.SingleOrDefaultAsync(new BookingByIdSpec(request.Id), cancellationToken);

        _ = booking ?? throw new NotFoundException(string.Format(_localizer["booking.notfound"], request.Id));

        var updatedBooking = booking.FlagConfirmationEmailSent();
        
        await _bookingRepository.UpdateAsync(updatedBooking, cancellationToken);

        return updatedBooking.Id;
    }
}