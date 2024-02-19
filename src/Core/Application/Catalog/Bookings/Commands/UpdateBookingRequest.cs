using System.Data;
using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Bookings.Commands;

public class UpdateBookingRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public string Description { get; set; } = default!;
    public decimal TotalAmount { get; set; } = default!;
    public string CurrencyCode { get; set; } = default!;
    public int ItemQuantity { get; set; } = default!;
    public bool IsPaid { get; set; } = default!;
    public DateTime BookingDate { get; set; } = default!;
    public string? GuestId { get; set; }
    public int ConcurrencyVersion { get; set; }

    public IList<UpdateBookingItemRequest> Items { get; set; } = [];
}

public class UpdateBookingRequestHandler : IRequestHandler<UpdateBookingRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Booking> _bookingRepository;
    private readonly IRepositoryFactory<TourDate> _tourDateRepository;
    private readonly IStringLocalizer<UpdateBookingRequestHandler> _localizer;

    public UpdateBookingRequestHandler(IRepositoryFactory<Booking> bookingRepository,
        IRepositoryFactory<TourDate> tourDateRepository,
        IStringLocalizer<UpdateBookingRequestHandler> localizer)
    {
        _bookingRepository = bookingRepository;
        _tourDateRepository = tourDateRepository;
        _localizer = localizer;
    }

    public async Task<DefaultIdType> Handle(UpdateBookingRequest request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.SingleOrDefaultAsync(new BookingByIdSpec(request.Id), cancellationToken);

        _ = booking ?? throw new NotFoundException(string.Format(_localizer["booking.notfound"], request.Id));

        if (booking.ConcurrencyVersion != request.ConcurrencyVersion)
        {
            // Handle concurrency conflict scenario
            throw new DBConcurrencyException("Booking has been updated by another user.");
        }

        // Update the booking properties
        booking.Update(
            request.Description,
            request.TotalAmount,
            request.CurrencyCode,
            request.ItemQuantity,
            request.IsPaid,
            request.BookingDate,
            request.GuestId);

        var bookingItems = new List<BookingItem>();

        // Update the booking item list
        foreach (var updateItemRequest in request.Items)
        {
            var bookingItem = booking.Items.FirstOrDefault(i => i.Id == updateItemRequest.Id);

            if (bookingItem == null)
            {
                if (updateItemRequest is {StartDate: not null, EndDate: not null, Amount: not null})
                {
                    // Create a new booking item if not found
                    bookingItem = new BookingItem(
                        updateItemRequest.StartDate.Value,
                        updateItemRequest.EndDate.Value,
                        updateItemRequest.Amount.Value,
                        updateItemRequest.RoomQuantity,
                        updateItemRequest.PropertyId,
                        updateItemRequest.TourId,
                        updateItemRequest.TourDateId,
                        updateItemRequest.CloudbedsReservationId,
                        updateItemRequest.CloudbedsPropertyId);

                    if (updateItemRequest is {TourDateId: not null, TourDate: not null})
                    {
                        var tourDate = await _tourDateRepository.GetByIdAsync(updateItemRequest.TourDateId.Value, cancellationToken);

                        if (tourDate == null)
                        {
                            throw new NotFoundException(_localizer["tourdate.notfound"]);
                        }

                        if (tourDate.ConcurrencyVersion != updateItemRequest.TourDate.ConcurrencyVersion)
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

                        // Add the new booking item to the booking
                        bookingItems.Add(bookingItem);
                    }
                }
            }
            else
            {
                if (bookingItem.ConcurrencyVersion != updateItemRequest.ConcurrencyVersion)
                {
                    // Handle concurrency conflict scenario
                    throw new DBConcurrencyException("Booking Item has been updated by another user.");
                }

                bookingItem.ConcurrencyVersion++;

                // Update the existing booking item properties
                bookingItem.Update(
                    updateItemRequest.StartDate,
                    updateItemRequest.EndDate,
                    updateItemRequest.Amount,
                    updateItemRequest.RoomQuantity,
                    updateItemRequest.PropertyId,
                    updateItemRequest.TourId,
                    updateItemRequest.TourDateId,
                    updateItemRequest.CloudbedsReservationId,
                    updateItemRequest.CloudbedsPropertyId);

                if (updateItemRequest.TourDateId.HasValue && updateItemRequest.TourDateId != bookingItem.TourDateId && updateItemRequest.TourDate != null)
                {
                    var tourDate = await _tourDateRepository.GetByIdAsync(updateItemRequest.TourDateId.Value, cancellationToken);

                    if (tourDate == null)
                    {
                        throw new NotFoundException(_localizer["tourdate.notfound"]);
                    }

                    if (tourDate.ConcurrencyVersion != updateItemRequest.TourDate.ConcurrencyVersion)
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

                // Add the new booking item to the booking
                bookingItems.Add(bookingItem);
            }

            if (updateItemRequest.Rooms?.Any() != true) continue;
            
            // Update the booking item rooms
            foreach (var updateRoomRequest in updateItemRequest.Rooms)
            {
                var room = bookingItem?.Rooms?.FirstOrDefault(r => r.Id == updateRoomRequest.Id);

                if (room == null && bookingItem != null)
                {
                    // Create a new room if not found
                    room = new BookingItemRoom(
                        bookingItem.Id,
                        updateRoomRequest.RoomName,
                        updateRoomRequest.Amount,
                        updateRoomRequest.Nights,
                        updateRoomRequest.CheckInDate,
                        updateRoomRequest.CheckOutDate,
                        updateRoomRequest.GuestFirstName,
                        updateRoomRequest.GuestLastName,
                        updateRoomRequest.CloudbedsGuestId);

                    // Add the new room to the booking item
                    bookingItem.Rooms?.Add(room);
                }
                else
                {
                    // Update the existing room properties
                    room?.Update(
                        updateRoomRequest.RoomName,
                        updateRoomRequest.Amount,
                        updateRoomRequest.Nights,
                        updateRoomRequest.CheckInDate,
                        updateRoomRequest.CheckOutDate,
                        updateRoomRequest.GuestFirstName,
                        updateRoomRequest.GuestLastName,
                        updateRoomRequest.CloudbedsGuestId);
                }
            }
        }

        booking.Items = bookingItems;
        booking.ConcurrencyVersion++;

        // Save the changes to the repository
        await _bookingRepository.UpdateAsync(booking, cancellationToken);

        return booking.Id;
    }
}