using Travaloud.Application.Basket.Dto;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Cloudbeds;

namespace Travaloud.Application.PaymentProcessing;

public interface IPaymentConfirmationService : ITransientService
{
    Task<CreateBookingRequest> CreateBookingRequest(DefaultIdType guestId, BasketModel basket);

    Task<bool> ProcessPropertyBookings(
        BasketModel basket,
        BookingDetailsDto booking,
        string cardToken,
        string paymentAuthorizationCode,
        ITenantWebsiteService tenantWebsiteService,
        ICloudbedsService cloudbedsService,
        IBookingsService bookingsService);
}