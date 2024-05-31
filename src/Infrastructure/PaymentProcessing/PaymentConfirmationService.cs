using MediatR;
using Microsoft.AspNetCore.Identity;
using Travaloud.Application.Basket.Dto;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Application.Cloudbeds;
using Travaloud.Application.Cloudbeds.Commands;
using Travaloud.Application.Cloudbeds.Queries;
using Travaloud.Application.PaymentProcessing;
using Travaloud.Application.PaymentProcessing.Commands;
using Travaloud.Infrastructure.Identity;
using Travaloud.Shared.Authorization;

namespace Travaloud.Infrastructure.PaymentProcessing;

public class PaymentConfirmationService : IPaymentConfirmationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISender _mediator;

    public PaymentConfirmationService(
        UserManager<ApplicationUser> userManager,
        ISender mediator)
    {
        _userManager = userManager;
        _mediator = mediator;
    }

    public async Task<CreateBookingRequest> CreateBookingRequest(DefaultIdType guestId, BasketModel basket,
        string stripeSessionId)
    {
        var propertyBookings = basket.Items.Where(x => x.PropertyId.HasValue);
        var tourBookings = basket.Items.Where(x => x.TourId.HasValue);

        var propertyBookingsModels = propertyBookings as BasketItemModel[] ?? propertyBookings.ToArray();
        var tourBookingsModels = tourBookings as BasketItemModel[] ?? tourBookings.ToArray();

        var bookingRequest = new CreateBookingRequest(
            $"Web Booking: {(propertyBookingsModels.Length != 0 ? $"{propertyBookingsModels.Length} Propert{(propertyBookingsModels.Length > 1 ? "ies" : "y")}" : "")}{(tourBookingsModels.Length != 0 ? $"{(propertyBookingsModels.Length > 0 ? " & " : "")}{tourBookingsModels.Length} Tour{(tourBookingsModels.Length > 1 ? "s" : "")}" : "")}",
            basket.Total,
            "USD",
            basket.Items.Count,
            false,
            DateTime.Now,
            guestId.ToString(),
            $"{basket.FirstName} {basket.Surname}",
            basket.Email,
            basket.AdditionalNotes,
            "Website"
        )
        {
            CreatedBy = basket.CreateId,
            Items = new List<CreateBookingItemRequest>(),
            StripeSessionId = stripeSessionId
        };

        foreach (var propertyBooking in basket.Items.Where(x => x.PropertyId.HasValue))
        {
            if (propertyBooking.Rooms == null) continue;
            if (propertyBooking.CheckInDateParsed == null || propertyBooking.CheckOutDateParsed == null) continue;

            var bookingItem = new CreateBookingItemRequest
            {
                StartDate = propertyBooking.CheckInDateParsed.Value,
                EndDate = propertyBooking.CheckOutDateParsed.Value,
                Amount = propertyBooking.Total,
                RoomQuantity = propertyBooking.Rooms.Count,
                PropertyId = propertyBooking.PropertyId,
                CloudbedsPropertyId = propertyBooking.CloudbedsPropertyId,
                Rooms = new List<CreateBookingItemRoomRequest>()
            };

            foreach (var room in propertyBooking.Rooms)
            {
                var numberOfNights = (int) (propertyBooking.CheckOutDateParsed.Value.Date -
                                            propertyBooking.CheckInDateParsed.Value.Date).TotalDays;

                bookingItem.Rooms.Add(new CreateBookingItemRoomRequest()
                {
                    RoomName = room.RoomTypeName,
                    Amount = room.RoomRate,
                    Nights = numberOfNights,
                    CheckInDate = propertyBooking.CheckInDateParsed.Value,
                    CheckOutDate = propertyBooking.CheckOutDateParsed.Value,
                    GuestFirstName = basket.FirstName!,
                    GuestLastName = basket.Surname!
                });
            }

            if (propertyBooking.Guests != null && propertyBooking.Guests.Any())
            {
                bookingItem.Guests = new List<BookingItemGuestRequest>();

                foreach (var guest in propertyBooking.Guests)
                {
                    var additionalGuestId = await CreateGuest(guest, basket);

                    if (additionalGuestId.HasValue)
                    {
                        bookingItem.Guests.Add(new BookingItemGuestRequest()
                        {
                            GuestId = additionalGuestId.Value.ToString()
                        });
                    }
                }
            }

            bookingRequest.Items.Add(bookingItem);
        }

        foreach (var tourBooking in basket.Items.Where(x => x.TourId.HasValue))
        {
            if (tourBooking.TourDates == null) continue;
            var tourDate = tourBooking.TourDates.First();

            var bookingItem = new CreateBookingItemRequest
            {
                StartDate = tourDate.StartDate,
                Amount = tourDate.Price,
                TourDateId = tourDate.DateId,
                TourId = tourDate.TourId,
                TourDate = new UpdateTourDateRequest
                {
                    Id = tourDate.DateId
                },
                GuestQuantity = tourDate.GuestQuantity,
                PickupLocation = tourBooking.PickupLocation
            };

            if (tourBooking.Guests != null && tourBooking.Guests.Any())
            {
                bookingItem.Guests = new List<BookingItemGuestRequest>();

                foreach (var guest in tourBooking.Guests)
                {
                    var additionalGuestId = await CreateGuest(guest, basket);

                    if (additionalGuestId.HasValue)
                    {
                        bookingItem.Guests.Add(new BookingItemGuestRequest()
                        {
                            GuestId = additionalGuestId.Value.ToString()
                        });
                    }
                }
            }

            bookingRequest.Items.Add(bookingItem);
        }

        return bookingRequest;
    }

    public async Task<bool> ProcessPropertyBookings(
        BasketModel basket,
        BookingDetailsDto booking,
        string cardToken,
        string paymentAuthorizationCode,
        ITenantWebsiteService tenantWebsiteService,
        ICloudbedsService cloudbedsService,
        IBookingsService bookingsService)
    {
        if (!basket.Items.Any(x => x.PropertyId.HasValue)) return true;
        {
            var properties = await tenantWebsiteService.GetProperties(CancellationToken.None);

            if (properties == null)
                return false;

            // Create cloudbeds reservation
            foreach (var basketItem in basket.Items.Where(x => x.PropertyId.HasValue))
            {
                var propertyDtos = properties as PropertyDto[] ?? properties.ToArray();
                var property = propertyDtos.FirstOrDefault(x =>
                    basketItem.PropertyId != null && x.Id == basketItem.PropertyId.Value);

                if (property == null)
                    return false;

                string? reservationId;

                try
                {
                    var createReservationResponse =
                        await cloudbedsService.CreateReservation(new CreateReservationRequest(basket,
                            basketItem, property.CloudbedsApiKey, cardToken, paymentAuthorizationCode));

                    if (!createReservationResponse.Success || booking.Items == null)
                        return false;

                    reservationId = createReservationResponse.ReservationID;

                    //Get reservation on Cloudbeds and check if any balance is to be handled, if so, manually make a payment for it.
                    var reservation = await _mediator.Send(new GetReservationRequest(
                        property.CloudbedsPropertyId,
                        reservationId,
                        property.CloudbedsApiKey));

                    if (reservation is {Success: true, Data.Balance: > 0})
                    {
                        var createReservationPaymentRequest = await cloudbedsService.CreateReservationPayment(
                            new CreateReservationPaymentRequest(property.CloudbedsPropertyId, reservationId,
                                reservation.Data.Balance, property.CloudbedsApiKey,
                                "Deducted value of Tours paid on Travaloud."));

                        if (!createReservationPaymentRequest.Success)
                        {
                            return false;
                        }
                    }
                }
                catch (Exception)
                {
                    return false;
                }

                var bookingItem = booking.Items.FirstOrDefault(x =>
                    basketItem is {CheckOutDateParsed: not null, CheckInDateParsed: not null} &&
                    x.PropertyId == basketItem.PropertyId &&
                    x.StartDate.Date == basketItem.CheckInDateParsed.Value.Date &&
                    x.EndDate.Date == basketItem.CheckOutDateParsed.Value.Date);

                if (bookingItem != null && !string.IsNullOrEmpty(reservationId))
                {
                    // Update booking item with reservation id
                    await bookingsService.UpdateBookingItemReservation(bookingItem.Id,
                        new UpdateBookingItemReservationIdRequest(bookingItem.Id, reservationId));
                }

                if (basketItem.Guests == null || !basketItem.Guests.Any()) continue;

                foreach (var guest in basketItem.Guests)
                {
                    await cloudbedsService.CreateReservationAdditionalGuest(new
                        CreateReservationAdditionalGuestRequest(
                            basketItem.CloudbedsPropertyId,
                            reservationId,
                            guest.FirstName,
                            guest.Surname,
                            guest.Gender,
                            guest.Email,
                            guest.PhoneNumber,
                            guest.DateOfBirth,
                            property.CloudbedsApiKey));
                }
            }
        }

        return true;
    }

    private async Task<DefaultIdType?> CreateGuest(BasketItemGuestModel guest, BasketModel basket)
    {
        DefaultIdType? guestId = null;

        var emailToUse = string.IsNullOrEmpty(guest.Email)
            ? $"{DefaultIdType.NewGuid()}@travaloudguest.com"
            : guest.Email;
        var existingUser = await _userManager.FindByEmailAsync(emailToUse);

        if (existingUser == null)
        {
            var user = new ApplicationUser
            {
                FirstName = guest.FirstName,
                LastName = guest.Surname,
                PhoneNumber = guest.PhoneNumber,
                Gender = guest.Gender,
                Nationality = guest.Nationality,
                DateOfBirth = guest.DateOfBirth,
                UserName = emailToUse,
                Email = emailToUse,
                SignUpDate = DateTime.Now,
                IsActive = true,
                EmailConfirmed = true,
                RefreshTokenExpiryTime = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, "P@55w0rd");

            if (!result.Succeeded) return guestId;

            guestId = DefaultIdType.Parse(user.Id);
            await _userManager.AddToRoleAsync(user, TravaloudRoles.Guest);
        }
        else
        {
            guestId = DefaultIdType.Parse(existingUser.Id);
        }

        return guestId;
    }

    public Task UpdatePaymentIntentDescription(UpdatePaymentIntentDescriptionRequest request)
    {
        return _mediator.Send(request);
    }
}