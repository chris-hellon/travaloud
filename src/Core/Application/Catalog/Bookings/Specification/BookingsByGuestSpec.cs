using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Bookings.Queries;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Specification;

public class BookingsByGuestSpec : EntitiesByPaginationFilterSpec<Booking, BookingDto>
{
    public BookingsByGuestSpec(GetGuestBookingsRequest request)
        : base(request)
    {
        Query
            .OrderByDescending(c => c.BookingDate, !request.HasOrderBy())
            .Include(x => x.Items)
            .ThenInclude(item => item.Tour)
            .Include(x => x.Items)
            .ThenInclude(item => item.TourDate)
            .Include(x => x.Items)
            .ThenInclude(item => item.Property)
            .Where(x => x.GuestId == request.GuestId && x.Items.Count > 0);
    }
}