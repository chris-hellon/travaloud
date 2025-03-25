using System.Data;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Application.Catalog.TourDates.Specification;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Application.Common.Utils;
using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Domain.Catalog.Tours;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Common.Extensions;

public static class BookingsExtensions
{
    public static void ProcessBookingItemGuests(this BookingItem bookingItem, IList<BookingItemGuestRequest>? request,
        DefaultIdType userId)
    {
        var guests = new List<BookingItemGuest>();

        if (request?.Any() == true)
        {
            foreach (var guestRequest in request)
            {
                var guest = bookingItem.Guests?.FirstOrDefault(i => i.Id == guestRequest.Id);

                if (guest == null)
                {
                    var newGuest = new BookingItemGuest(DefaultIdType.Parse(guestRequest.GuestId));
                    guests.Add(newGuest);
                }
                else
                {
                    guests.Add(guest);
                }
            }
        }

        var directionsToRemove = bookingItem.Guests?
            .Where(existingRoom => guests.All(newRoom => newRoom.Id != existingRoom.Id))
            .ToList();

        if (directionsToRemove != null && directionsToRemove.Count != 0)
        {
            foreach (var direction in directionsToRemove)
            {
                direction.DomainEvents.Add(EntityDeletedEvent.WithEntity(direction));
                direction.FlagAsDeleted(userId);
                guests.Add(direction);
            }
        }

        bookingItem.Guests = guests;
    }

    public static async Task ProcessBookingItems(this Booking updatedBooking, Booking booking,
        IList<UpdateBookingItemRequest>? request, DefaultIdType userId, IRepositoryFactory<TourDate> tourDateRepository,
        CancellationToken cancellationToken)
    {
        var bookingItems = new List<BookingItem>();
        var tourDatesToUpdate = new List<TourDate>();

        // Update the booking item list
        foreach (var updateItemRequest in request)
        {
            var currentBookingItem = booking.Items.FirstOrDefault(i => i.Id == updateItemRequest.Id);

            // If a new Booking item
            if (currentBookingItem == null)
            {
                if (updateItemRequest is not {TourDateId: not null, TourDate: not null})
                    throw new CustomException("No Tour Date provided.");

                var tourDate =
                    await tourDateRepository.FirstOrDefaultAsync(
                        new TourDateByIdSpec(updateItemRequest.TourDateId.Value), cancellationToken);

                if (tourDate == null)
                {
                    throw new NotFoundException("No tour date found.");
                }

                if (tourDate.AvailableSpaces < updateItemRequest.GuestQuantity)
                {
                    throw new DBConcurrencyException(
                        "The request Tour Date no longer has enough spaces available. Please refresh the page and try again.");
                }

                if (updateItemRequest is not {StartDate: not null, Amount: not null})
                    throw new CustomException("No Start or Amount provided.");

                var endDate = DateTimeUtils.CalculateEndDate(tourDate.StartDate, tourDate.TourPrice.DayDuration,
                    tourDate.TourPrice.NightDuration, tourDate.TourPrice.HourDuration);

                // Create a new booking item if not found
                var newBookingItem = new BookingItem(
                    updateItemRequest.StartDate.Value,
                    endDate,
                    updateItemRequest.Amount.Value,
                    updateItemRequest.RoomQuantity,
                    updateItemRequest.PropertyId,
                    updateItemRequest.TourId,
                    updateItemRequest.TourDateId,
                    updateItemRequest.CloudbedsReservationId,
                    updateItemRequest.CloudbedsPropertyId,
                    updateItemRequest.OtherPickupLocation ?? updateItemRequest.PickupLocation,
                    updateItemRequest.WaiverSigned,
                    updateItemRequest.TourCategoryId,
                    booking.CreatedBy);

                newBookingItem.ProcessBookingItemGuests(updateItemRequest.Guests, userId);

                if (tourDate.AvailableSpaces > 0)
                {
                    tourDate.AvailableSpaces -= updateItemRequest.GuestQuantity;
                    tourDate.ConcurrencyVersion++;

                    lock (tourDatesToUpdate)
                    {
                        if (tourDatesToUpdate.All(x => x.Id != tourDate.Id))
                            tourDatesToUpdate.Add(tourDate);
                    }

                    var sameTourDates = await tourDateRepository.ListAsync(
                        new SameTourDatesSpec(updateItemRequest.TourId.Value, tourDate.StartDate, tourDate.EndDate,
                            tourDate.Id), cancellationToken);

                    if (sameTourDates.Count != 0)
                    {
                        sameTourDates = sameTourDates.Select(x =>
                        {
                            if (x.AvailableSpaces <= 0) return x;

                            x.AvailableSpaces -= updateItemRequest.GuestQuantity;
                            x.ConcurrencyVersion++;

                            return x;
                        }).ToList();

                        lock (tourDatesToUpdate)
                        {
                            foreach (var sameTourDate in sameTourDates.Where(sameTourDate =>
                                         tourDatesToUpdate.All(x => x.Id != sameTourDate.Id)))
                            {
                                tourDatesToUpdate.Add(sameTourDate);
                            }
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException("There are no spaces available for this date.");
                }

                // Add the new booking item to the booking
                bookingItems.Add(newBookingItem);

                if (updateItemRequest.Rooms?.Any() != true) continue;

                newBookingItem.AddRooms(updateItemRequest);
            }
            else
            {
                if (currentBookingItem.ConcurrencyVersion != updateItemRequest.ConcurrencyVersion)
                {
                    // Handle concurrency conflict scenario
                    throw new DBConcurrencyException("Booking Item has been updated by another user.");
                }

                currentBookingItem.ConcurrencyVersion++;

                // Update the existing booking item properties
                var newBookingItem = currentBookingItem.Update(
                    updateItemRequest.StartDate,
                    updateItemRequest.EndDate,
                    updateItemRequest.Amount,
                    updateItemRequest.RoomQuantity,
                    updateItemRequest.PropertyId,
                    updateItemRequest.TourId,
                    updateItemRequest.TourDateId,
                    updateItemRequest.CloudbedsReservationId,
                    updateItemRequest.CloudbedsPropertyId,
                    null,
                    updateItemRequest.OtherPickupLocation ?? updateItemRequest.PickupLocation,
                    updateItemRequest.WaiverSigned,
                    updateItemRequest.TourCategoryId,
                    booking.CreatedBy,
                    updateItemRequest.CheckedIn,
                    updateItemRequest.Cancelled,
                    updateItemRequest.NoShow);

                var currentGuestCount = currentBookingItem.Guests != null && currentBookingItem.Guests.Any()
                    ? currentBookingItem.Guests.Count + 1
                    : 1;

                newBookingItem.ProcessBookingItemGuests(updateItemRequest.Guests, userId);

                var newGuestCount = newBookingItem.Guests?.Where(x => !x.DeletedOn.HasValue).Count() + 1;
                var currentAndNewGuestCountDifference = currentGuestCount - newGuestCount;

                //If selecting a new date
                if (updateItemRequest.TourDateId.HasValue &&
                    updateItemRequest.TourDateId != currentBookingItem.TourDate?.Id &&
                    updateItemRequest.TourDate != null)
                {
                    var currentTourDate = currentBookingItem.TourDate;
                    currentTourDate.AvailableSpaces += currentGuestCount;
                    currentTourDate.ConcurrencyVersion++;

                    lock (tourDatesToUpdate)
                    {
                        if (tourDatesToUpdate.All(x => x.Id != currentTourDate.Id))
                            tourDatesToUpdate.Add(currentTourDate);
                        
                    }

                    var sameTourDates = await tourDateRepository.ListAsync(
                        new SameTourDatesSpec(updateItemRequest.TourId.Value, currentTourDate.StartDate,
                            currentTourDate.EndDate,
                            currentTourDate.Id,
                            tourDatesToUpdate.Select(x => x.Id).ToList()), cancellationToken);

                    if (sameTourDates.Count != 0)
                    {
                        sameTourDates = sameTourDates.Select(x =>
                        {
                            if (x.AvailableSpaces <= 0) return x;

                            x.AvailableSpaces += currentGuestCount;
                            x.ConcurrencyVersion++;

                            return x;
                        }).ToList();

                        lock (tourDatesToUpdate)
                        {
                            foreach (var sameTourDate in sameTourDates.Where(sameTourDate =>
                                         tourDatesToUpdate.All(x => x.Id != sameTourDate.Id)))
                            {
                                tourDatesToUpdate.Add(sameTourDate);
                            }
                        }
                    }

                    var tourDate =
                        await tourDateRepository.FirstOrDefaultAsync(
                            new TourDateByIdSpec(updateItemRequest.TourDateId.Value), cancellationToken);

                    if (tourDate == null)
                    {
                        throw new NotFoundException("No tour date found.");
                    }

                    if (tourDate.AvailableSpaces < updateItemRequest.GuestQuantity)
                    {
                        throw new DBConcurrencyException(
                            "The request Tour Date no longer has enough spaces available. Please refresh the page and try again.");
                    }

                    var endDate = DateTimeUtils.CalculateEndDate(tourDate.StartDate, tourDate.TourPrice.DayDuration,
                        tourDate.TourPrice.NightDuration, tourDate.TourPrice.HourDuration);
                    newBookingItem.SetEndDate(endDate);

                    if (tourDate.AvailableSpaces > 0)
                    {
                        tourDate.AvailableSpaces -= updateItemRequest.GuestQuantity;
                        tourDate.ConcurrencyVersion++;

                        var updatedTourDate = tourDate.Update(
                            tourDate.StartDate,
                            tourDate.EndDate,
                            tourDate.AvailableSpaces,
                            tourDate.PriceOverride,
                            tourDate.TourId,
                            tourDate.TourPriceId);

                        newBookingItem.TourDate = updatedTourDate;
                    }
                    else
                    {
                        throw new InvalidOperationException("There are no spaces available for this Tour Date.");
                    }
                }
                else if (updateItemRequest.TourDateId.HasValue)
                {
                    var tourDate = currentBookingItem.TourDate ??
                                   await tourDateRepository.GetByIdAsync(updateItemRequest.TourDateId.Value,
                                       cancellationToken);

                    if (tourDate == null)
                    {
                        throw new NotFoundException("Tour date not found.");
                    }

                    if (currentAndNewGuestCountDifference.HasValue && currentAndNewGuestCountDifference != 0)
                    {
                        tourDate.AvailableSpaces += currentAndNewGuestCountDifference.Value;
                        tourDate.ConcurrencyVersion++;

                        lock (tourDatesToUpdate)
                        {
                            if (tourDatesToUpdate.All(x => x.Id != tourDate.Id))
                                tourDatesToUpdate.Add(tourDate);
                        }

                        var sameTourDates = await tourDateRepository.ListAsync(
                            new SameTourDatesSpec(updateItemRequest.TourId.Value, tourDate.StartDate, tourDate.EndDate,
                                tourDate.Id,
                                tourDatesToUpdate.Select(x => x.Id).ToList()), cancellationToken);

                        if (sameTourDates.Count != 0)
                        {
                            sameTourDates = sameTourDates.Select(x =>
                            {
                                if (x.AvailableSpaces <= 0) return x;

                                x.AvailableSpaces += currentAndNewGuestCountDifference.Value;
                                x.ConcurrencyVersion++;

                                return x;
                            }).ToList();

                            lock (tourDatesToUpdate)
                            {
                                foreach (var sameTourDate in sameTourDates.Where(sameTourDate =>
                                             tourDatesToUpdate.All(x => x.Id != sameTourDate.Id)))
                                {
                                    tourDatesToUpdate.Add(sameTourDate);
                                }
                            }
                        }
                    }
                }

                // Add the new booking item to the booking
                bookingItems.Add(newBookingItem);

                if (updateItemRequest.Rooms?.Any() != true) continue;

                newBookingItem.AddRooms(updateItemRequest);
            }
        }

        var itemsToRemove = booking.Items?
            .Where(existingItem => bookingItems.All(newItem => newItem.Id != existingItem.Id))
            .ToList();

        if (itemsToRemove != null && itemsToRemove.Count != 0)
        {
            foreach (var bookingItem in itemsToRemove)
            {
                bookingItem.DomainEvents.Add(EntityDeletedEvent.WithEntity(bookingItem));
                bookingItem.FlagAsDeleted(userId);
                bookingItems.Add(bookingItem);

                var tourDate = bookingItem.TourDate ??
                               await tourDateRepository.GetByIdAsync(bookingItem.TourDateId.Value, cancellationToken);

                if (tourDate == null) continue;

                var currentGuestCount = bookingItem.Guests != null && bookingItem.Guests.Any()
                    ? bookingItem.Guests.Count + 1
                    : 1;

                tourDate.AvailableSpaces += currentGuestCount;

                lock (tourDatesToUpdate)
                {
                    if (tourDatesToUpdate.All(x => x.Id != tourDate.Id))
                        tourDatesToUpdate.Add(tourDate);
                }

                var sameTourDates = await tourDateRepository.ListAsync(
                    new SameTourDatesSpec(bookingItem.TourId.Value, tourDate.StartDate, tourDate.EndDate,
                        tourDate.Id,
                        tourDatesToUpdate.Select(x => x.Id).ToList()), cancellationToken);

                if (sameTourDates.Count != 0)
                {
                    sameTourDates = sameTourDates.Select(x =>
                    {
                        if (x.AvailableSpaces <= 0) return x;

                        x.AvailableSpaces += currentGuestCount;
                        x.ConcurrencyVersion++;

                        return x;
                    }).ToList();

                    lock (tourDatesToUpdate)
                    {
                        foreach (var sameTourDate in sameTourDates.Where(sameTourDate =>
                                     tourDatesToUpdate.All(x => x.Id != sameTourDate.Id)))
                        {
                            tourDatesToUpdate.Add(sameTourDate);
                        }
                    }
                }
            }
        }

        if (tourDatesToUpdate.Any())
            await tourDateRepository.UpdateRangeAsync(tourDatesToUpdate, cancellationToken);

        booking.Items = bookingItems;
    }

    private static void AddRooms(this BookingItem bookingItem, UpdateBookingItemRequest updateItemRequest)
    {
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
}