using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Travaloud.Application.Basket.Dto;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.PaymentProcessing;
using Travaloud.Application.PaymentProcessing.Commands;

namespace Travaloud.Application.Checkout.Commands;

public class CreatePaymentLinkRequest : IRequest<string>
{
    public BasketModel? Basket { get; set; }
    public DefaultIdType? GuestId { get; set; }
}

internal class CreatePaymentLinkRequestHandler : IRequestHandler<CreatePaymentLinkRequest, string>
{
    private readonly IStripeService _stripeService;
    private readonly IBookingsService _bookingsService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreatePaymentLinkRequestHandler(IStripeService stripeService, IBookingsService bookingsService, IHttpContextAccessor httpContextAccessor)
    {
        _stripeService = stripeService;
        _bookingsService = bookingsService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> Handle(CreatePaymentLinkRequest request, CancellationToken cancellationToken)
    {
        if (request.Basket == null || !request.GuestId.HasValue)
            throw new NotFoundException("No basket or guest id provided.");
        
        var propertyBookings = request.Basket.Items.Where(x => x.PropertyId.HasValue);
        var tourBookings =  request.Basket.Items.Where(x => x.TourId.HasValue);

        var propertyBookingsModels = propertyBookings as BasketItemModel[] ?? propertyBookings.ToArray();
        var tourBookingsModels = tourBookings as BasketItemModel[] ?? tourBookings.ToArray();
        
        var bookingRequest = new CreateBookingRequest(
            $"{ request.Basket.FirstName} { request.Basket.Surname}: {(propertyBookingsModels.Length != 0 ? $"{propertyBookingsModels.Length} x Propert{(propertyBookingsModels.Length > 1 ? "ies" : "y")}" : "")} {(tourBookingsModels.Length != 0 ? $" & {tourBookingsModels.Length} x Tour{(tourBookingsModels.Length > 1 ? "s" : "")}" : "")}",
            request.Basket.Total,
            "USD",
            request.Basket.Items.Count,
            false,
            DateTime.Now,
            request.GuestId.ToString())
        {
            Items = new List<CreateBookingItemRequest>()
        };

        foreach (var propertyBooking in request.Basket.Items.Where(x => x.PropertyId.HasValue))
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
                var numberOfNights = (int)( propertyBooking.CheckOutDateParsed.Value.Date - propertyBooking.CheckInDateParsed.Value.Date).TotalDays;
                
                bookingItem.Rooms.Add(new CreateBookingItemRoomRequest()
                {
                    RoomName = room.RoomTypeName,
                    Amount = room.RoomRate,
                    Nights = numberOfNights,
                    CheckInDate = propertyBooking.CheckInDateParsed.Value,
                    CheckOutDate = propertyBooking.CheckOutDateParsed.Value,
                    GuestFirstName = request.Basket.FirstName!,
                    GuestLastName = request.Basket.Surname!
                });
            }
            
            bookingRequest.Items.Add(bookingItem);
        }
            
        var bookingId = await _bookingsService.CreateAsync(bookingRequest);

        var booking = await _bookingsService.GetAsync(bookingId);
        
        var url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}";
        var fullUrl = _httpContextAccessor.HttpContext.Request.GetDisplayUrl();

        var stripeSession = await _stripeService.CreateStripeSession(new CreateStripeSessionRequest(
            bookingId,
            booking.InvoiceId,
            $"{url}/payment-confirmation/{bookingId.ToString()}",
            fullUrl,
            request.Basket,
            request.Basket.Email
        ));

        return stripeSession.Url;
    }
}