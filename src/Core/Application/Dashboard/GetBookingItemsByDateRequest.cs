using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Application.Identity.Users;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Dashboard;

public class GetBookingItemsByDateRequest : PaginationFilter, IRequest<PaginationResponse<BookingExportDto>>
{
    public DateTime? TourStartDate { get; set; }
    public DateTime? TourEndDate { get; set; }
    public DefaultIdType? TourId { get; set; }
    public DefaultIdType? TourDateId { get; set; }
    public string? Description { get; set; }
    public IEnumerable<UserDetailsDto>? Guests { get; set; }
}

internal class GetBookingItemsByDateRequestHandler : IRequestHandler<GetBookingItemsByDateRequest, PaginationResponse<BookingExportDto>>
{
    private readonly IRepositoryFactory<BookingItem> _repository;
    private readonly IUserService _userService;
    
    public GetBookingItemsByDateRequestHandler(IRepositoryFactory<BookingItem> repository, 
        IUserService userService)
    {
        _repository = repository;
        _userService = userService;
    }

    public async Task<PaginationResponse<BookingExportDto>> Handle(GetBookingItemsByDateRequest request, CancellationToken cancellationToken)
    {
        var exportBookingsRequest = new ExportBookingsRequest()
        {
            TourStartDate = request.TourStartDate.HasValue ? request.TourStartDate.Value.Date + new TimeSpan(00, 00, 00) : null,
            TourEndDate = request.TourEndDate.HasValue ? request.TourEndDate.Value.Date + new TimeSpan(23, 59, 59) : null,
            Description = request.Description,
            IsTourBookings = true,
            TourId = request.TourId,
            TourDateId = request.TourDateId
        };
        var spec = new ExportBookingsSpec(exportBookingsRequest);
        
        var todaysTours = await _repository.ListAsync(spec, cancellationToken);
        var parsedTodaysTours = new List<BookingExportDto>();

        if (todaysTours != null)
        {
            foreach (var item in todaysTours)
            {
                if (item.BookingGuestId == null || request.Guests == null) continue;
                var guest = request.Guests.FirstOrDefault(g => g.Id == item.BookingGuestId);

                if (guest == null) continue;
            
                item.GuestName = $"{guest.FirstName} {guest.LastName}";
                item.GuestGender = guest.Gender;
                item.GuestDateOfBirth = guest.DateOfBirth?.Date;
                item.GuestNationality = guest.Nationality;
                item.GuestPassportNumber = guest.PassportNumber;

                parsedTodaysTours.Add(item);

                if (item.Guests == null || !item.Guests.Any()) continue;
                {
                    foreach (var additionalGuest in item.Guests)
                    {
                        var additionalGuestMatch =
                            request.Guests.FirstOrDefault(x => DefaultIdType.Parse(x.Id) == additionalGuest.GuestId);

                        if (additionalGuestMatch != null)
                        {
                            parsedTodaysTours.Add(new BookingExportDto()
                            {
                                Amount = item.Amount,
                                BookingCurrencyCode = item.BookingCurrencyCode,
                                BookingBookingDate = item.BookingBookingDate,
                                BookingInvoiceId = item.BookingInvoiceId,
                                BookingIsPaid = item.BookingIsPaid,
                                StartDate = item.StartDate,
                                EndDate = item.EndDate,
                                TourId = item.TourId,
                                TourName = item.TourName,
                                BookingGuestId = item.BookingGuestId,
                                GuestName = $"{additionalGuestMatch.FirstName} {additionalGuestMatch.LastName}",
                                GuestGender = additionalGuestMatch.Gender,
                                GuestDateOfBirth = additionalGuestMatch.DateOfBirth?.Date,
                                GuestNationality = additionalGuestMatch.Nationality,
                                GuestPassportNumber = additionalGuestMatch.PassportNumber,
                                PickupLocation = item.PickupLocation
                            });
                        }
                    }
                }
            }
        }
       
        
        return new PaginationResponse<BookingExportDto>(parsedTodaysTours, parsedTodaysTours.Count, request.PageNumber, request.PageSize);
    }
}