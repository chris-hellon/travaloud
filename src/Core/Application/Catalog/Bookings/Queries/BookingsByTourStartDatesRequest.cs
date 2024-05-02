using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Queries;

public class BookingsByTourStartDatesRequest : IRequest<IList<DefaultIdType?>?>
{
    public IEnumerable<DefaultIdType> TourDateIds { get; set; }

    public BookingsByTourStartDatesRequest(IEnumerable<DefaultIdType> tourDateIds)
    {
        TourDateIds = tourDateIds;
    }
}

internal class BookingsByTourStartDatesRequestHandler : IRequestHandler<BookingsByTourStartDatesRequest, IList<DefaultIdType?>>
{
    private readonly IRepositoryFactory<BookingItem> _repository;

    public BookingsByTourStartDatesRequestHandler(IRepositoryFactory<BookingItem> repository)
    {
        _repository = repository;
    }

    public async Task<IList<DefaultIdType?>> Handle(BookingsByTourStartDatesRequest request, CancellationToken cancellationToken)
    {
        var existingBookings = await _repository.ListAsync(new BookingsByDatesSpec(request.TourDateIds), cancellationToken);
        var existingBookingsDateIds = existingBookings.Select(x => x.TourDateId);

        return existingBookingsDateIds.ToList();
    }
}