using System.Data;

namespace Travaloud.Application.Catalog.Bookings.Commands;

public class UpdateBookingItemGuestStatusRequest : IRequest
{
    public int BookingInvoiceId { get; set; }
    public DefaultIdType TourId { get; set; }
    public DateTime TourStartDate { get; set; }
    public DefaultIdType TourDateId { get; set; }
    public string GuestId { get; set; }
    public bool CheckedIn { get; set; }
    public bool NoShow { get; set; }
    public bool Cancelled { get; set; }

    public UpdateBookingItemGuestStatusRequest(int bookingInvoiceId, DefaultIdType tourId, DateTime tourStartDate, string guestId, DefaultIdType tourDateId, bool checkedIn, bool noShow, bool cancelled)
    {
        BookingInvoiceId = bookingInvoiceId;
        TourId = tourId;
        TourDateId = tourDateId;
        TourStartDate = tourStartDate;
        GuestId = guestId;
        CheckedIn = checkedIn;
        NoShow = noShow;
        Cancelled = cancelled;
    }
}

internal class UpdateBookingItemGuestStatusRequestHandler : IRequestHandler<UpdateBookingItemGuestStatusRequest>
{
    private readonly IDapperRepository _dapperRepository;

    public UpdateBookingItemGuestStatusRequestHandler(IDapperRepository dapperRepository)
    {
        _dapperRepository = dapperRepository;
    }
    
    public async Task Handle(UpdateBookingItemGuestStatusRequest request, CancellationToken cancellationToken)
    {
        await _dapperRepository.ExecuteAsync("UpdateBookingGuestStatus", new
        {
            request.BookingInvoiceId,
            request.TourId,
            request.TourStartDate,
            request.GuestId,
            request.CheckedIn,
            request.NoShow,
            request.Cancelled,
            request.TourDateId
        }, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
    }
}