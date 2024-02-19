using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Specification;

public class ExportBookingsSpec : EntitiesByBaseFilterSpec<BookingItem, BookingExportDto>
{
    public ExportBookingsSpec(ExportBookingsRequest request)
        : base(request)
    {
        Query
            .OrderBy(c => c.StartDate)
            .Include(x => x.Booking)
            .Include(x => x.Tour)
            .Include(x => x.TourDate)
            .Where(x =>
                (request.TourId.HasValue || (x.TourId.HasValue && x.TourId.Value == request.TourId))
                && (string.IsNullOrEmpty(request.Description) || x.Booking.Description.Equals(request.Description))
                && (!request.BookingStartDate.HasValue || !request.BookingEndDate.HasValue
                                                       || (x.Booking.BookingDate >= request.BookingStartDate.Value && x.Booking.BookingDate <= request.BookingEndDate.Value))
                && (!request.TourStartDate.HasValue || !request.TourEndDate.HasValue
                                                    || (x.StartDate >= request.TourStartDate.Value && x.StartDate <= request.TourEndDate.Value)));
    }
}