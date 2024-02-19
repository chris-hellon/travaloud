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
    public DefaultIdType? TourId { get; set; }
    public List<UserDetailsDto>? Guests { get; set; }
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

        list = list.ConvertAll(x =>
        {
            if (x.BookingGuestId == null || request.Guests == null) return x;
            var guest = request.Guests.FirstOrDefault(g => g.Id == DefaultIdType.Parse(x.BookingGuestId));

            if (guest == null) return x;
            
            x.GuestName = $"{guest.FirstName} {guest.LastName}";
            x.GuestGender = guest.Gender;
            x.GuestDateOfBirth = guest.DateOfBirth;
            x.GuestNationality = guest.Nationality;
            x.GuestPassportNumber = guest.PassportNumber;

            return x;
        });

        return _excelWriter.WriteToStream(list);
    }
}

