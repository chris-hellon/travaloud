using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Bookings.Queries;
using Travaloud.Application.Common.Exporters;

namespace Travaloud.Application.Catalog.Interfaces;

public interface IBookingsService : ITransientService
{
    Task<PaginationResponse<BookingDto>> SearchAsync(SearchBookingsRequest request);

    Task<BookingDetailsDto> GetAsync(DefaultIdType id);

    Task<IEnumerable<BookingDto>> GetGuestBookingsAsync(GetGuestBookingsRequest request);

    Task<bool> GetBookingsByDateAsync(BookingsByTourStartDateRequest request);
    
    Task<IList<DefaultIdType?>> GetBookingsByDatesAsync(BookingsByTourStartDatesRequest request);
    
    Task<DefaultIdType?> CreateAsync(CreateBookingRequest request);

    Task<string> CreateStaffTourBookingQrCodeUrl(CreateStaffTourBookingQrCodeUrlRequest request);
    
    Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateBookingRequest request);

    Task<DefaultIdType?> FlagBookingAsPaidAsync(DefaultIdType id, FlagBookingAsPaidRequest request);

    Task<DefaultIdType> UpdateBookingItemReservation(DefaultIdType id, UpdateBookingItemReservationIdRequest request);

    Task<DefaultIdType> DeleteAsync(DefaultIdType id);

    Task<DefaultIdType> DeleteItemAsync(DefaultIdType id);

    Task<FileResponse> ExportAsync(ExportBookingsRequest filter);

    Task<PaginationResponse<StaffBookingDto>> StaffBookingsByDateRange(StaffBookingsByDateRangeRequest request);

    Task<FileResponse> ExportStaffBookingsAsync(ExportStaffBookingsRequest filter);

    Task FlagBookingStripeStatus(FlagBookingStripeStatusRequest request);

    Task<bool> RefundBooking(RefundBookingRequest request);

    Task<DefaultIdType?> FlagBookingConfirmationEmailAsync(DefaultIdType id, FlagBookingConfirmationEmailRequest request);
}