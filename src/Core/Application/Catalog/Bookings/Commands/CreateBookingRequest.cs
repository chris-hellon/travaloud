using System.Data;
using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Bookings.Commands;

public class CreateBookingRequest : IRequest<DefaultIdType>
{
    public string Description { get; set; } = default!;
    public decimal TotalAmount { get; set; } = default!;
    public string CurrencyCode { get; set; } = default!;
    public int ItemQuantity { get; set; } = default!;
    public bool IsPaid { get; set; } = default!;
    public DateTime BookingDate { get; set; } = default!;
    public string? GuestId { get; set; }
    public int ConcurrencyVersion { get; set; } = default!;
    public IList<CreateBookingItemRequest> Items { get; set; } = default!;
}

public class CreateBookingRequestHandler : IRequestHandler<CreateBookingRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Booking> _bookingRepository;
    private readonly IRepositoryFactory<TourDate> _tourDateRepository;
    private readonly IStringLocalizer<CreateBookingRequestHandler> _localizer;

    public CreateBookingRequestHandler(IRepositoryFactory<Booking> bookingRepository,
        IRepositoryFactory<TourDate> tourDateRepository,
        IStringLocalizer<CreateBookingRequestHandler> localizer)
    {
        _bookingRepository = bookingRepository;
        _tourDateRepository = tourDateRepository;
        _localizer = localizer;
    }

    public async Task<DefaultIdType> Handle(CreateBookingRequest request, CancellationToken cancellationToken)
    {
        // Create the booking
        var booking = new Booking(
            request.Description,
            request.TotalAmount,
            request.CurrencyCode,
            request.ItemQuantity,
            request.IsPaid,
            request.BookingDate,
            request.GuestId);

        // Create booking items and associate them with the booking
        if (request.Items?.Any() == true)
        {
            var bookingItems = new List<BookingItem>();
            foreach (var itemRequest in request.Items)
            {
                var bookingItem = new BookingItem(
                    itemRequest.StartDate,
                    itemRequest.EndDate,
                    itemRequest.Amount,
                    itemRequest.RoomQuantity,
                    itemRequest.PropertyId,
                    itemRequest.TourId,
                    itemRequest.TourDateId,
                    itemRequest.CloudbedsReservationId,
                    itemRequest.CloudbedsPropertyId);

                if (itemRequest is {TourDateId: not null, TourDate: not null})
                {
                    var tourDate = await _tourDateRepository.GetByIdAsync(itemRequest.TourDateId.Value, cancellationToken);

                    if (tourDate == null)
                    {
                        throw new NotFoundException(_localizer["tourdate.notfound"]);
                    }

                    if (tourDate.ConcurrencyVersion != itemRequest.TourDate.ConcurrencyVersion)
                    {
                        throw new DBConcurrencyException("The TourDate has been modified by another user. Please refresh and try again.");
                    }

                    if (tourDate.AvailableSpaces > 0)
                    {
                        tourDate.AvailableSpaces--;
                        tourDate.ConcurrencyVersion++;
                        await _tourDateRepository.UpdateAsync(tourDate, cancellationToken);
                    }
                    else
                    {
                        throw new InvalidOperationException(_localizer["tourdate.nospaces"]);
                    }
                }

                // Create booking item rooms and associate them with the booking item
                if (itemRequest.Rooms?.Any() == true)
                {
                    var bookingItemRooms = itemRequest.Rooms.Select(roomRequest => 
                        new BookingItemRoom(bookingItem.Id, roomRequest.RoomName, roomRequest.Amount,
                            roomRequest.Nights, roomRequest.CheckInDate, roomRequest.CheckOutDate,
                            roomRequest.GuestFirstName, roomRequest.GuestLastName, roomRequest.CloudbedsGuestId))
                        .ToList();

                    bookingItem.Rooms = bookingItemRooms;
                }

                bookingItems.Add(bookingItem);
            }

            booking.Items = bookingItems;
        }

        await _bookingRepository.AddAsync(booking, cancellationToken);

        return booking.Id;
    }
}