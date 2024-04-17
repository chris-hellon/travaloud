using System.Data;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Application.Common.Utils;
using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Bookings.Commands;

public class CreateBookingRequest : IRequest<DefaultIdType?>
{
    public string Description { get; set; } = default!;
    public decimal TotalAmount { get; set; } 
    public string CurrencyCode { get; set; } = default!;
    public int ItemQuantity { get; set; }
    public bool IsPaid { get; set; }
    public DateTime BookingDate { get; set; }
    public string? GuestId { get; set; }
    public int ConcurrencyVersion { get; set; }
    public IList<CreateBookingItemRequest> Items { get; set; } = default!;

    public CreateBookingRequest()
    {
        
    }
    
    public CreateBookingRequest(string description, decimal totalAmount, string currencyCode, int itemQuantity, bool isPaid, DateTime bookingDate, string? guestId, int concurrencyVersion, IList<CreateBookingItemRequest> items)
    {
        Description = description;
        TotalAmount = totalAmount;
        CurrencyCode = currencyCode;
        ItemQuantity = itemQuantity;
        IsPaid = isPaid;
        BookingDate = bookingDate;
        GuestId = guestId;
        ConcurrencyVersion = concurrencyVersion;
        Items = items;
    }
    
    public CreateBookingRequest(string description, decimal totalAmount, string currencyCode, int itemQuantity, bool isPaid, DateTime bookingDate, string? guestId)
    {
        Description = description;
        TotalAmount = totalAmount;
        CurrencyCode = currencyCode;
        ItemQuantity = itemQuantity;
        IsPaid = isPaid;
        BookingDate = bookingDate;
        GuestId = guestId;
    }
}

public class CreateBookingRequestHandler : IRequestHandler<CreateBookingRequest, DefaultIdType?>
{
    private readonly IRepositoryFactory<Booking> _bookingRepository;
    private readonly IRepositoryFactory<TourDate> _tourDateRepository;
    private readonly IStringLocalizer<CreateBookingRequestHandler> _localizer;
    private readonly ICurrentUser _currentUser;
    
    public CreateBookingRequestHandler(IRepositoryFactory<Booking> bookingRepository,
        IRepositoryFactory<TourDate> tourDateRepository,
        IStringLocalizer<CreateBookingRequestHandler> localizer, ICurrentUser currentUser)
    {
        _bookingRepository = bookingRepository;
        _tourDateRepository = tourDateRepository;
        _localizer = localizer;
        _currentUser = currentUser;
    }

    public async Task<DefaultIdType?> Handle(CreateBookingRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.GuestId))
            throw new CustomException("A Guest must be selected.");
        
        // Create the booking
        var booking = new Booking(
            request.Description,
            request.TotalAmount,
            request.CurrencyCode,
            request.ItemQuantity,
            request.IsPaid,
            request.BookingDate,
            request.GuestId, 
            null);

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

                var userId = _currentUser.GetUserId();
                bookingItem.ProcessBookingItemGuests(itemRequest.Guests, userId);
                
                if (itemRequest is {TourDateId: not null, TourDate: not null})
                {
                    var tourDate = await _tourDateRepository.FirstOrDefaultAsync(new TourDateByIdSpec(itemRequest.TourDateId.Value), cancellationToken);

                    if (tourDate == null)
                    {
                        throw new NotFoundException("The specified Tour Date can not be found.");
                    }

                    itemRequest.GuestQuantity ??= 1;
                    
                    if (itemRequest.GuestQuantity != null && tourDate.AvailableSpaces < itemRequest.GuestQuantity.Value)
                    {
                        throw new DBConcurrencyException("The request Tour Date no longer has enough spaces available. Please refresh the page and try again.");
                    }

                    var endDate = DateTimeUtils.CalculateEndDate(tourDate.StartDate, tourDate.TourPrice.DayDuration, tourDate.TourPrice.NightDuration, tourDate.TourPrice.HourDuration);
                    bookingItem.SetEndDate(endDate);
                    
                    if (tourDate.AvailableSpaces > 0)
                    {
                        tourDate.AvailableSpaces -= itemRequest.GuestQuantity!.Value;
                        tourDate.ConcurrencyVersion++;
                        await _tourDateRepository.UpdateAsync(tourDate, cancellationToken);
                    }
                    else
                    {
                        throw new InvalidOperationException("There are no spaces available on this Tour Date. Please change your selection and try again.");
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