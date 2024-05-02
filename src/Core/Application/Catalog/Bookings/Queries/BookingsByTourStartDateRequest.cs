using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Queries;

public class BookingsByTourStartDateRequest : IRequest<bool>
{
    public DefaultIdType TourDateId { get; set; }

    public BookingsByTourStartDateRequest(DefaultIdType tourDateId)
    {
        TourDateId = tourDateId;
    }
}

internal class BookingsByTourStartDateRequestHandler : IRequestHandler<BookingsByTourStartDateRequest, bool>
{
    private readonly IRepositoryFactory<BookingItem> _repository;

    public BookingsByTourStartDateRequestHandler(IRepositoryFactory<BookingItem> repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(BookingsByTourStartDateRequest request, CancellationToken cancellationToken)
    {
        var existingBooking = await _repository.SingleOrDefaultAsync(new BookingsByDateSpec(request.TourDateId), cancellationToken);

        return existingBooking != null;
    }
}