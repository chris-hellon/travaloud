using System.Data;
using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Application.Catalog.TourDates.Specification;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Application.Common.Utils;
using Travaloud.Application.Identity.Users;
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
    public string? GuestName { get; set; }
    public string? GuestEmail { get; set; }
    public int ConcurrencyVersion { get; set; }
    public bool WaiverSigned { get; set; }
    public IList<CreateBookingItemRequest> Items { get; set; } = default!;
    public string? CreatedBy { get; set; }
    public UserDetailsDto? Guest { get; set; }
    public string? AdditionalNotes { get; set; }
    public string? BookingSource { get; set; }
    public string? StripeSessionId { get; set; }
    public decimal? AmountOutstanding { get; set; }
    public bool IsWebsite { get; set; }

    public CreateBookingRequest()
    {
    }

    public CreateBookingRequest(string description,
        decimal totalAmount,
        string currencyCode,
        int itemQuantity,
        bool isPaid,
        DateTime bookingDate,
        string? guestId,
        string? guestName,
        string? guestEmail,
        int concurrencyVersion,
        string? additionalNotes,
        string? bookingSource,
        IList<CreateBookingItemRequest> items)
    {
        Description = description;
        TotalAmount = totalAmount;
        CurrencyCode = currencyCode;
        ItemQuantity = itemQuantity;
        IsPaid = isPaid;
        BookingDate = bookingDate;
        GuestId = guestId;
        GuestName = guestName;
        ConcurrencyVersion = concurrencyVersion;
        Items = items;
        GuestEmail = guestEmail;
        AdditionalNotes = additionalNotes;
        BookingSource = bookingSource;
    }

    public CreateBookingRequest(string description,
        decimal totalAmount,
        string currencyCode,
        int itemQuantity,
        bool isPaid,
        DateTime bookingDate,
        string? guestId,
        string? guestName,
        string? guestEmail,
        string? additionalNotes,
        string? bookingSource)
    {
        Description = description;
        TotalAmount = totalAmount;
        CurrencyCode = currencyCode;
        ItemQuantity = itemQuantity;
        IsPaid = isPaid;
        BookingDate = bookingDate;
        GuestId = guestId;
        GuestName = guestName;
        GuestEmail = guestEmail;
        AdditionalNotes = additionalNotes;
        BookingSource = bookingSource;
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
        if (!request.IsWebsite && string.IsNullOrEmpty(request.GuestId))
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
            request.StripeSessionId,
            request.WaiverSigned,
            request.GuestName,
            request.GuestEmail,
            request.AdditionalNotes,
            request.BookingSource,
            null,
            request.AmountOutstanding);

        if (!string.IsNullOrEmpty(request.CreatedBy))
        {
            booking.OverrideCreatedBy = true;
            booking.CreatedBy = DefaultIdType.Parse(request.CreatedBy);
        }
        else if (booking.CreatedBy == DefaultIdType.Empty && request.GuestId != null)
            booking.CreatedBy = DefaultIdType.Parse(request.GuestId);

        var tourDatesToUpdate = new List<TourDate>();

        // Create booking items and associate them with the booking
        if (request.Items?.Any() == true)
        {
            var bookingItemsTasks = request.Items.Select(async itemRequest =>
            {
                try
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
                        itemRequest.CloudbedsPropertyId,
                        itemRequest.OtherPickupLocation ?? itemRequest.PickupLocation,
                        itemRequest.WaiverSigned,
                        itemRequest.TourCategoryId,
                        booking.CreatedBy);

                    if (!string.IsNullOrEmpty(request.CreatedBy))
                    {
                        bookingItem.OverrideCreatedBy = true;
                        bookingItem.CreatedBy = DefaultIdType.Parse(request.CreatedBy);
                    }

                    var userId = _currentUser.GetUserId();
                    bookingItem.ProcessBookingItemGuests(itemRequest.Guests, userId);

                    if (itemRequest is {TourDateId: not null, TourDate: not null})
                    {
                        var tourDate = await _tourDateRepository.FirstOrDefaultAsync(
                            new TourDateByIdSpec(itemRequest.TourDateId.Value), cancellationToken);

                        if (tourDate == null)
                        {
                            throw new NotFoundException(
                                $"The specified Tour Date with ID {itemRequest.TourDateId.Value} cannot be found.");
                        }

                        itemRequest.GuestQuantity ??= 1;

                        if (itemRequest.GuestQuantity != null &&
                            tourDate.AvailableSpaces < itemRequest.GuestQuantity.Value)
                        {
                            throw new DBConcurrencyException(
                                $"The requested Tour Date {tourDate.StartDate.ToLongDateString()} no longer has enough spaces available. Please refresh the page and try again.");
                        }

                        var endDate = DateTimeUtils.CalculateEndDate(
                            tourDate.StartDate,
                            tourDate.TourPrice.DayDuration,
                            tourDate.TourPrice.NightDuration,
                            tourDate.TourPrice.HourDuration);

                        bookingItem.SetEndDate(endDate);

                        if (tourDate.AvailableSpaces > 0)
                        {
                            tourDate.AvailableSpaces -= itemRequest.GuestQuantity!.Value;
                            tourDate.ConcurrencyVersion++;

                            lock (tourDatesToUpdate)
                            {
                                tourDatesToUpdate.Add(tourDate);
                            }
                            
                            var sameTourDates = await _tourDateRepository.ListAsync(
                                new SameTourDatesSpec(itemRequest.TourId.Value, tourDate.StartDate, tourDate.EndDate,
                                    tourDate.Id), cancellationToken);

                            if (sameTourDates.Count != 0)
                            {
                                sameTourDates = sameTourDates.Select(x =>
                                {
                                    if (x.AvailableSpaces > 0)
                                        x.AvailableSpaces -= itemRequest.GuestQuantity!.Value;
                                    return x;
                                }).ToList();
                                
                                lock (tourDatesToUpdate)
                                {
                                    tourDatesToUpdate.AddRange(sameTourDates);
                                }
                            }

                        }
                        else
                        {
                            throw new InvalidOperationException(
                                $"There are no spaces available for Tour Date {tourDate.StartDate.ToLongDateString()}. Please change your selection and try again.");
                        }
                    }

                    if (itemRequest.Rooms?.Any() == true)
                    {
                        bookingItem.Rooms = itemRequest.Rooms.Select(roomRequest =>
                            new BookingItemRoom(bookingItem.Id, roomRequest.RoomName, roomRequest.Amount,
                                roomRequest.Nights, roomRequest.CheckInDate, roomRequest.CheckOutDate,
                                roomRequest.GuestFirstName, roomRequest.GuestLastName,
                                roomRequest.CloudbedsGuestId)).ToList();
                    }

                    return (object) bookingItem;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            });
            
            var results = await Task.WhenAll(bookingItemsTasks);
            var exceptions = results.OfType<Exception>().ToList();

            if (exceptions.Count != 0)
            {
                throw new AggregateException("One or more errors occurred while creating booking items.", exceptions);
            }

            booking.Items = results.OfType<BookingItem>().ToList();
        }

        if (request.Items?.Any() == true)
        {
            var tasks = request.Items
                .Where(x => x is {TourDateId: not null, TourDate: not null})
                .Select(async itemRequest =>
                {
                    try
                    {
                        var tourDate =
                            await _tourDateRepository.FirstOrDefaultAsync(
                                new TourDateByIdWithoutPriceSpec(itemRequest.TourDateId!.Value), cancellationToken);

                        if (tourDate == null)
                        {
                            throw new NotFoundException(
                                $"The specified Tour Date with ID {itemRequest.TourDateId.Value} cannot be found.");
                        }

                        if (itemRequest.GuestQuantity != null &&
                            tourDate.AvailableSpaces < itemRequest.GuestQuantity.Value)
                        {
                            throw new DBConcurrencyException(
                                $"The requested Tour Date {tourDate.StartDate.ToLongDateString()} no longer has enough spaces available. Please refresh the page and try again.");
                        }
                    }
                    catch (Exception ex)
                    {
                        return ex;
                    }

                    return null;
                });

            var results = await Task.WhenAll(tasks);
            var exceptions = results.Where(ex => ex != null).ToList();

            if (exceptions is {Count: > 0})
            {
                throw new AggregateException("One or more errors occurred while checking Tour Dates availability.",
                    exceptions!);
            }
        }

        await _tourDateRepository.UpdateRangeAsync(tourDatesToUpdate, cancellationToken);
        await _bookingRepository.AddAsync(booking, cancellationToken);

        return booking.Id;
    }
}