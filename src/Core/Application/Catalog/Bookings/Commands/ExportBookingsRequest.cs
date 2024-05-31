using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Application.Common.Exporters;
using Travaloud.Application.Identity.Users;
using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Shared.Authorization;

namespace Travaloud.Application.Catalog.Bookings.Commands;

public class ExportBookingsRequest : BaseFilter, IRequest<Stream>
{
    public string? Description { get; set; }
    public DateTime? BookingStartDate { get; set; }
    public DateTime? BookingEndDate { get; set; }
    public DateTime? TourStartDate { get; set; }
    public DateTime? TourEndDate { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public DefaultIdType? TourId { get; set; }
    public DefaultIdType? TourDateId { get; set; }
    public DefaultIdType? PropertyId { get; set; }
    public List<UserDetailsDto>? Guests { get; set; }
    public IEnumerable<DefaultIdType>? TourIds { get; set; }
    public bool IsTourBookings { get; set; }
    public bool HideRefunded { get; set; }
}

public class ExportBookingsRequestHandler : IRequestHandler<ExportBookingsRequest, Stream>
{
    private readonly IRepositoryFactory<BookingItem> _repository;
    private readonly IExcelWriter _excelWriter;
    private readonly IUserService _userService;
    private readonly ICurrentUser _currentUser;

    public ExportBookingsRequestHandler(
        IRepositoryFactory<BookingItem> repository, 
        IExcelWriter excelWriter, 
        IUserService userService, 
        ICurrentUser currentUser)
    {
        _repository = repository;
        _excelWriter = excelWriter;
        _userService = userService;
        _currentUser = currentUser;
    }

    public async Task<Stream> Handle(ExportBookingsRequest request, CancellationToken cancellationToken)
    {
        var isSupplier = _currentUser.IsInRole(TravaloudRoles.Supplier);
        var spec = new ExportBookingsSpec(request);

        var list = await _repository.ListAsync(spec, cancellationToken);
        var parsedList = new List<BookingExportDto>();

        if (request.HideRefunded)
            list = list.Where(x => !x.BookingRefunded.HasValue || !x.BookingRefunded.Value).ToList();
        
        var staffIds = list.Select(x => x.CreatedBy.ToString()).ToList();

        var staff = await _userService.SearchAsync(staffIds, CancellationToken.None);

        if (staff.Count != 0) 
        {
            var bookings = list.Select(x =>
            {
                var staffMember = staff.FirstOrDefault(s => s.Id == x.CreatedBy);

                x.BookingStaffName = staffMember == null || staffMember.Id.ToString() == x.BookingGuestId ? "Direct from Website"  : $"{staffMember.FirstName} {staffMember.LastName}";
                   
                return x;
            });

            list = bookings.ToList();
        }
        
        foreach (var item in list)
        {
            if (item.BookingGuestId == null || request.Guests == null) continue;
            var guest = request.Guests.FirstOrDefault(g => g.Id == item.BookingGuestId);

            if (guest == null) continue;
            
            item.GuestName = $"{guest.FirstName} {guest.LastName}";
            item.GuestGender = guest.Gender;
            item.GuestDateOfBirth = guest.DateOfBirth?.Date;
            item.GuestNationality = guest.Nationality?.Length == 2 ? guest.Nationality.TwoLetterCodeToCountry() : guest.Nationality;
            item.GuestPassportNumber = guest.PassportNumber;
            item.GuestId = guest.Id;
            item.Amount = isSupplier ? 0 : item.Amount;

            parsedList.Add(item);

            if (item.Guests == null || !item.Guests.Any()) continue;
            {
                parsedList.AddRange(item.Guests.Select(additionalGuest => request.Guests.FirstOrDefault(g => Guid.Parse(g.Id) == additionalGuest.GuestId))
                .Select(additionalGuestMatch => new BookingExportDto()
                {
                    BookingAdditionalNotes = item.BookingAdditionalNotes,
                    Amount = isSupplier ? 0 : item.Amount,
                    BookingCurrencyCode = item.BookingCurrencyCode,
                    BookingBookingDate = item.BookingBookingDate,
                    BookingInvoiceId = item.BookingInvoiceId,
                    BookingIsPaid = item.BookingIsPaid,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    TourId = item.TourId,
                    TourName = item.TourName,
                    BookingGuestId = additionalGuestMatch.Id,
                    GuestId = item.GuestId,
                    GuestName = $"{additionalGuestMatch.FirstName} {additionalGuestMatch.LastName}",
                    GuestGender = additionalGuestMatch.Gender,
                    GuestDateOfBirth = additionalGuestMatch.DateOfBirth?.Date,
                    GuestNationality = additionalGuestMatch.Nationality?.Length == 2 ? additionalGuestMatch.Nationality.TwoLetterCodeToCountry() : additionalGuestMatch.Nationality,
                    GuestPassportNumber = additionalGuestMatch.PassportNumber,
                    PickupLocation = item.PickupLocation,
                    WaiverSigned = item.WaiverSigned,
                    BookingBookingSource = item.BookingBookingSource,
                    BookingStaffName = item.BookingStaffName
                }));
            }
        }
        
        return _excelWriter.WriteToStream(parsedList);
    }
}

