using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Common.Extensions;

public static class BookingsExtensions
{
    public static void ProcessBookingItemGuests(this BookingItem bookingItem, IList<BookingItemGuestRequest>? request, DefaultIdType userId)
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
    
}