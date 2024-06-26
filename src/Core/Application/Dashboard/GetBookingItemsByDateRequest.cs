using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Application.Identity.Users;
using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Shared.Authorization;

namespace Travaloud.Application.Dashboard;

public class GetBookingItemsByDateRequest : PaginationFilter, IRequest<PaginationResponse<BookingExportDto>>
{
    public DateTime? TourStartDate { get; set; }
    public DateTime? TourEndDate { get; set; }
    public DateTime? BookingStartDate { get; set; }
    public DateTime? BookingEndDate { get; set; }
    public DefaultIdType? TourId { get; set; }
    public DefaultIdType? TourDateId { get; set; }
    public IEnumerable<DefaultIdType>? TourIds { get; set; }
    public string? Description { get; set; }
    public IEnumerable<UserDetailsDto>? Guests { get; set; }
    public bool HideRefunded { get; set; } = true;
}

internal class GetBookingItemsByDateRequestHandler : IRequestHandler<GetBookingItemsByDateRequest, PaginationResponse<BookingExportDto>>
{
    private readonly IRepositoryFactory<BookingItem> _repository;
    private readonly IUserService _userService;
    private readonly ICurrentUser _currentUser;
    
    public GetBookingItemsByDateRequestHandler(
        IRepositoryFactory<BookingItem> repository, 
        IUserService userService, 
        ICurrentUser currentUser)
    {
        _repository = repository;
        _userService = userService;
        _currentUser = currentUser;
    }

    public async Task<PaginationResponse<BookingExportDto>> Handle(GetBookingItemsByDateRequest request, CancellationToken cancellationToken)
    {
        var isSupplier = _currentUser.IsInRole(TravaloudRoles.Supplier);

        if (isSupplier)
        {
            var userClaims = await _userService.GetUserClaims(new GetUserClaimsRequest()
            {
                ClaimType = "SupplierTour",
                UserId = _currentUser.GetUserId().ToString()
            }, CancellationToken.None);

            request.TourIds = userClaims.Select(x => DefaultIdType.Parse(x.ClaimValue)).ToArray();
        }
        
        var exportBookingsRequest = new ExportBookingsRequest()
        {
            TourStartDate = request.TourStartDate.HasValue ? request.TourStartDate.Value.Date + new TimeSpan(00, 00, 00) : null,
            TourEndDate = request.TourEndDate.HasValue ? request.TourEndDate.Value.Date + new TimeSpan(23, 59, 59) : null,
            BookingStartDate = request.BookingStartDate.HasValue ? request.BookingStartDate.Value.Date + new TimeSpan(00, 00, 00) : null,
            BookingEndDate = request.BookingEndDate.HasValue ? request.BookingEndDate.Value.Date + new TimeSpan(23, 59, 59) : null,
            Description = request.Description,
            IsTourBookings = true,
            TourId = request.TourId,
            TourDateId = request.TourDateId,
            TourIds = request.TourIds,
            HideRefunded = true
        };
        var spec = new ExportBookingsSpec(exportBookingsRequest);
        
        var todaysTours = await _repository.ListAsync(spec, cancellationToken);
        var parsedTodaysTours = new List<BookingExportDto>();

        if (request.HideRefunded)
            todaysTours = todaysTours.Where(x => !x.BookingRefunded.HasValue || !x.BookingRefunded.Value).ToList();
        
        if (todaysTours == null)
            return new PaginationResponse<BookingExportDto>(parsedTodaysTours, parsedTodaysTours.Count,
                request.PageNumber, request.PageSize);
        
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
            item.GuestId = guest.Id;
            item.Amount = isSupplier ? 0 : item.Amount;

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
                            Amount = isSupplier ? 0 : item.Amount,
                            BookingCurrencyCode = item.BookingCurrencyCode,
                            BookingBookingDate = item.BookingBookingDate,
                            BookingInvoiceId = item.BookingInvoiceId,
                            BookingIsPaid = item.BookingIsPaid,
                            StartDate = item.StartDate,
                            EndDate = item.EndDate,
                            TourId = item.TourId,
                            TourName = item.TourName,
                            BookingGuestId = item.BookingGuestId,
                            GuestId = additionalGuestMatch.Id,
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
        
        var filteredTours = parsedTodaysTours;

        if (!string.IsNullOrEmpty(request.Keyword))
        {
            var searchTerm = request.Keyword.ToLower();
            filteredTours = filteredTours.Where(t =>
                    t.BookingInvoiceId.ToString().Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    t.GuestName!.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    t.TourName!.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
            ).ToList();
        }
        
        if (request.OrderBy is {Length: > 0})
        {
            foreach (var orderByField in request.OrderBy)
            {
                filteredTours = orderByField switch
                {
                    "Reference" => filteredTours.ToList(),
                    "Reference Ascending" => filteredTours.OrderBy(t => t.BookingInvoiceId).ToList(),
                    "Reference Descending" => filteredTours.OrderByDescending(t => t.BookingInvoiceId).ToList(),
                    "GuestName" => filteredTours.ToList(),
                    "GuestName Ascending" => filteredTours.OrderBy(t => t.GuestName).ToList(),
                    "GuestName Descending" => filteredTours.OrderByDescending(t => t.GuestName).ToList(),
                    "TourName" => filteredTours.ToList(),
                    "TourName Ascending" => filteredTours.OrderBy(t => t.TourName).ToList(),
                    "TourName Descending" => filteredTours.OrderByDescending(t => t.TourName).ToList(),
                    "StartDate" => filteredTours.ToList(),
                    "StartDate Ascending" => filteredTours.OrderBy(t => t.StartDate).ToList(),
                    "StartDate Descending" => filteredTours.OrderByDescending(t => t.StartDate).ToList(),
                    "EndDate" => filteredTours.ToList(),
                    "EndDate Ascending" => filteredTours.OrderBy(t => t.EndDate).ToList(),
                    "EndDate Descending" => filteredTours.OrderByDescending(t => t.EndDate).ToList(),
                    "BookingIsPaid" => filteredTours.ToList(),
                    "BookingIsPaid Ascending" => filteredTours.OrderBy(t => t.BookingIsPaid).ToList(),
                    "BookingIsPaid Descending" => filteredTours.OrderByDescending(t => t.BookingIsPaid).ToList(),
                    "BookingWaiverSigned" => filteredTours.ToList(),
                    "BookingWaiverSigned Ascending" => filteredTours.OrderBy(t => t.WaiverSigned).ToList(),
                    "BookingWaiverSigned Descending" => filteredTours.OrderByDescending(t => t.WaiverSigned)
                        .ToList(),
                    _ => filteredTours
                };
            }
        }

        // Apply pagination after ordering
        var totalCount = filteredTours.Count;
        var skip = (request.PageNumber - 1) * request.PageSize;
        filteredTours = filteredTours.Skip(skip).Take(request.PageSize).ToList();

        return new PaginationResponse<BookingExportDto>(filteredTours, totalCount, request.PageNumber, request.PageSize);

    }
}