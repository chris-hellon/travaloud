using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Commands;

public class UpdateBookingItemReservationIdRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public string? ReservationId { get; set; }

    public UpdateBookingItemReservationIdRequest(DefaultIdType id, string? reservationId)
    {
        Id = id;
        ReservationId = reservationId;
    }
}

internal class UpdateBookingItemReservationIdRequestHandler : IRequestHandler<UpdateBookingItemReservationIdRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<BookingItem> _repository;
    private readonly IStringLocalizer<UpdateBookingItemReservationIdRequestHandler> _localizer;

    public UpdateBookingItemReservationIdRequestHandler(IRepositoryFactory<BookingItem> repository, IStringLocalizer<UpdateBookingItemReservationIdRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<DefaultIdType> Handle(UpdateBookingItemReservationIdRequest request, CancellationToken cancellationToken)
    {
        var bookingItem = await _repository.SingleOrDefaultAsync(new BookingItemByIdSpec(request.Id), cancellationToken);
        
        _ = bookingItem ?? throw new NotFoundException(string.Format(_localizer["bookingItem.notfound"], request.Id));

        var updatedBookingItem = bookingItem.SetReservationId(request.ReservationId);
        
        await _repository.UpdateAsync(updatedBookingItem, cancellationToken);

        return updatedBookingItem.Id;
    }
}