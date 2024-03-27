using System.Data;
using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Commands;

public class FlagBookingAsPaidRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
}

internal class FlagBookingAsPaidRequestHandler : IRequestHandler<FlagBookingAsPaidRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Booking> _bookingRepository;
    private readonly IStringLocalizer<FlagBookingAsPaidRequestHandler> _localizer;

    public FlagBookingAsPaidRequestHandler(IRepositoryFactory<Booking> bookingRepository, IStringLocalizer<FlagBookingAsPaidRequestHandler> localizer)
    {
        _bookingRepository = bookingRepository;
        _localizer = localizer;
    }

    public async Task<DefaultIdType> Handle(FlagBookingAsPaidRequest request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.SingleOrDefaultAsync(new BookingByIdSpec(request.Id), cancellationToken);

        _ = booking ?? throw new NotFoundException(string.Format(_localizer["booking.notfound"], request.Id));

        var updatedBooking = booking.FlagBookingAsPaid();
        
        await _bookingRepository.UpdateAsync(updatedBooking, cancellationToken);

        return updatedBooking.Id;
    }
}