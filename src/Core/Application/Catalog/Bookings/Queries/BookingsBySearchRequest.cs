using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Queries;

public class BookingsBySearchRequest : EntitiesByPaginationFilterSpec<Booking, BookingDto>
{
    public BookingsBySearchRequest(SearchBookingsRequest request)
        : base(request)
    {
        if (request.IsTours)
        {
            Query
                .OrderByDescending(c => c.BookingDate, !request.HasOrderBy())
                .Include(x => x.Items)
                .ThenInclude(item => item.Tour)
                .Include(x => x.Items)
                .ThenInclude(item => item.TourDate)
                .Include(x => x.Items)
                .ThenInclude(item => item.Guests)
                .Where(x =>
                    (request.TourId.HasValue
                        ? x.Items.Any(i => i.TourId.HasValue && i.TourId.Value == request.TourId)
                        : x.Items.Any(i => i.TourId.HasValue))
                    && (request.Description == null || x.Description.Equals(request.Description))
                    && (!request.BookingStartDate.HasValue || !request.BookingEndDate.HasValue
                                                           || (x.BookingDate >= request.BookingStartDate.Value && x.BookingDate <= request.BookingEndDate.Value))
                    && (!request.TourStartDate.HasValue || !request.TourEndDate.HasValue
                                                        || x.Items.Any(i => i.StartDate >= request.TourStartDate.Value && i.StartDate <= request.TourEndDate.Value)));
        }
        else
        {
            Query
                .OrderByDescending(c => c.BookingDate, !request.HasOrderBy())
                .Include(x => x.Items)
                .ThenInclude(item => item.Property)
                .Include(x => x.Items)
                .ThenInclude(item => item.Guests)
                .Where(x =>
                    (request.PropertyId.HasValue
                        ? x.Items.Any(i => i.PropertyId.HasValue && i.PropertyId.Value == request.PropertyId)
                        : x.Items.Any(i => i.PropertyId.HasValue))
                    && (request.Description == null || x.Description.Equals(request.Description))
                    && (!request.BookingStartDate.HasValue || !request.BookingEndDate.HasValue
                                                           || (x.BookingDate >= request.BookingStartDate.Value && x.BookingDate <= request.BookingEndDate.Value)));
        }
    }
}