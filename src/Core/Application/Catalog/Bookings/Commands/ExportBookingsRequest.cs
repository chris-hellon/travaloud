using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Application.Common.Exporters;
using Travaloud.Application.Identity.Users;
using Travaloud.Domain.Catalog.Bookings;

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
    public DefaultIdType? PropertyId { get; set; }
    public List<UserDetailsDto>? Guests { get; set; }
    public bool IsTourBookings { get; set; }
}

public class ExportBookingsRequestHandler : IRequestHandler<ExportBookingsRequest, Stream>
{
    private readonly IRepositoryFactory<BookingItem> _repository;
    private readonly IExcelWriter _excelWriter;

    public ExportBookingsRequestHandler(IRepositoryFactory<BookingItem> repository, IExcelWriter excelWriter)
    {
        _repository = repository;
        _excelWriter = excelWriter;
    }

    public async Task<Stream> Handle(ExportBookingsRequest request, CancellationToken cancellationToken)
    {
        var spec = new ExportBookingsSpec(request);

        var list = await _repository.ListAsync(spec, cancellationToken);
        var parsedList = new List<BookingExportDto>();

        foreach (var item in list)
        {
            if (item.BookingGuestId == null || request.Guests == null) continue;
            var guest = request.Guests.FirstOrDefault(g => g.Id == DefaultIdType.Parse(item.BookingGuestId));

            if (guest == null) continue;
            
            item.GuestName = $"{guest.FirstName} {guest.LastName}";
            item.GuestGender = guest.Gender;
            item.GuestDateOfBirth = guest.DateOfBirth?.Date;
            item.GuestNationality = guest.Nationality;
            item.GuestPassportNumber = guest.PassportNumber;

            parsedList.Add(item);

            if (item.Guests == null || !item.Guests.Any()) continue;
            {
                parsedList.AddRange(item.Guests.Select(additionalGuest => request.Guests.FirstOrDefault(g => g.Id == additionalGuest.GuestId))
                .Select(additionalGuestMatch => new BookingExportDto()
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
                    GuestPassportNumber = additionalGuestMatch.PassportNumber
                }));
            }
        }
        
        // list = list.ConvertAll(x =>
        // {
        //     if (x.BookingGuestId == null || request.Guests == null) return x;
        //     var guest = request.Guests.FirstOrDefault(g => g.Id == DefaultIdType.Parse(x.BookingGuestId));
        //
        //     if (guest == null) return x;
        //     
        //     x.GuestName = $"{guest.FirstName} {guest.LastName}";
        //     x.GuestGender = guest.Gender;
        //     x.GuestDateOfBirth = guest.DateOfBirth;
        //     x.GuestNationality = guest.Nationality;
        //     x.GuestPassportNumber = guest.PassportNumber;
        //
        //     if (x.Guests != null && x.Guests.Any())
        //     {
        //         foreach (var additionalGuest in x.Guests)
        //         {
        //             var additionalGuestMatch = request.Guests.FirstOrDefault(g => g.Id == additionalGuest.GuestId);
        //             
        //             var guestModel = new BookingExportDto()
        //             {
        //                 Amount = x.Amount,
        //                 BookingBookingDate = x.BookingBookingDate,
        //                 BookingInvoiceId = x.BookingInvoiceId,
        //                 BookingIsPaid = x.BookingIsPaid,
        //                 StartDate = x.StartDate,
        //                 EndDate = x.EndDate,
        //                 TourId = x.TourId,
        //                 TourName = x.TourName,
        //                 BookingGuestId = x.BookingGuestId,
        //                 GuestName = $"{additionalGuestMatch.FirstName} {additionalGuestMatch.LastName}",
        //                 GuestGender = additionalGuestMatch.Gender,
        //                 GuestDateOfBirth = additionalGuestMatch.DateOfBirth,
        //                 GuestNationality = additionalGuestMatch.Nationality,
        //                 GuestPassportNumber = additionalGuestMatch.PassportNumber
        //             };
        //         }
        //
        //     }
        //     
        //     return x;
        // });

        return _excelWriter.WriteToStream(parsedList);
    }
}

