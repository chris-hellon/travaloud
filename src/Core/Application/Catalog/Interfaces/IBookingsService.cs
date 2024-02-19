using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Bookings.Queries;
using Travaloud.Application.Common.Exporters;

namespace Travaloud.Application.Catalog.Interfaces;

public interface IBookingsService : ITransientService
{
    Task<PaginationResponse<BookingDto>> SearchAsync(SearchBookingsRequest request);

    Task<BookingDetailsDto> GetAsync(DefaultIdType id);

    Task<DefaultIdType> CreateAsync(CreateBookingRequest request);

    Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateBookingRequest request);

    Task<DefaultIdType> DeleteAsync(DefaultIdType id);

    Task<DefaultIdType> DeleteItemAsync(DefaultIdType id);

    Task<FileResponse> ExportAsync(ExportBookingsRequest filter);
}