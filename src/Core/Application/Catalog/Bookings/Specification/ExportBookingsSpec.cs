using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Dashboard;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Specification;

public class ExportBookingsSpec : EntitiesByBaseFilterSpec<BookingItem, BookingExportDto>
{
    public ExportBookingsSpec(ExportBookingsRequest request)
        : base(request)
    {
        Query
            .OrderBy(c => c.StartDate)
            .Include(x => x.Guests)
            .Include(x => x.Booking)
            .Include(x => x.Tour)
            .Include(x => x.TourDate)
            .Where(
                x => x.TourId.HasValue &&
                     (request.TourId.HasValue ? x.TourId.Value == request.TourId : !request.TourId.HasValue),
                condition: request.IsTourBookings)
            .Where(x => request.TourIds != null && x.TourId.HasValue && request.TourIds.Contains(x.TourId.Value), condition: request.TourIds != null)
            .Where(x => x.TourDateId == request.TourDateId, condition: request.TourDateId.HasValue)
            .Where(
                x => x.PropertyId.HasValue && (request.PropertyId.HasValue
                    ? x.PropertyId.Value == request.PropertyId.Value
                    : !request.PropertyId.HasValue), condition: !request.IsTourBookings)
            .Where(x => x.Booking.Description.Contains(request.Keyword) || x.Booking.InvoiceId.ToString() == request.Keyword || x.Booking.GuestName.Contains(request.Keyword),
                condition: !string.IsNullOrEmpty(request.Keyword))
            .Where(
                x => x.Booking.BookingDate >= request.BookingStartDate.Value &&
                     x.Booking.BookingDate <= request.BookingEndDate.Value,
                condition: request is {BookingStartDate: not null, BookingEndDate: not null})
            .Where(
                x => x.StartDate >= request.TourStartDate.Value &&
                     x.StartDate <= request.TourEndDate.Value,
                condition: request is {TourStartDate: not null, TourEndDate: not null})
            .Where(
                x => x.StartDate >= request.CheckInDate.Value &&
                     x.StartDate <= request.CheckOutDate.Value,
                condition: request is {CheckInDate: not null, CheckOutDate: not null});
    }
}