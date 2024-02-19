using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Queries;

public class BookingsByGuestRequest : EntitiesByPaginationFilterSpec<Booking, BookingDto>
{
    public BookingsByGuestRequest(GetGuestBookingsRequest request)
        : base(request)
    {
        Query
            .OrderBy(c => c.BookingDate, !request.HasOrderBy())
            .Include(x => x.Items)
            .ThenInclude(item => item.Tour)
            .Include(x => x.Items)
            .ThenInclude(item => item.TourDate)
            .Include(x => x.Items)
            .ThenInclude(item => item.Property)
            .Where(x => x.GuestId == request.GuestId);
    }
}