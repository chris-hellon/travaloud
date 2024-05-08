using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Cloudbeds;
using Travaloud.Application.Cloudbeds.Commands;
using Travaloud.Application.PaymentProcessing;
using Travaloud.Application.PaymentProcessing.Commands;
using Travaloud.Application.PaymentProcessing.Queries;
using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Application.Catalog.Bookings.Commands;

public class RefundBookingRequest : IRequest<bool>
{
    public BookingDetailsDto Booking { get; set; }

    public RefundBookingRequest(BookingDetailsDto booking)
    {
        Booking = booking;
    }
}

internal class RefundBookingRequestHandler : IRequestHandler<RefundBookingRequest, bool>
{
    private readonly ICloudbedsService _cloudbedsService;
    private readonly IStripeService _stripeService;
    private readonly IRepositoryFactory<Booking> _repository;
    private readonly IRepositoryFactory<Property> _propertyRepository;

    public RefundBookingRequestHandler(ICloudbedsService cloudbedsService,
        IStripeService stripeService,
        IRepositoryFactory<Booking> repository,
        IRepositoryFactory<Property> propertyRepository)
    {
        _cloudbedsService = cloudbedsService;
        _stripeService = stripeService;
        _repository = repository;
        _propertyRepository = propertyRepository;
    }

    public async Task<bool> Handle(RefundBookingRequest request, CancellationToken cancellationToken)
    {
        var booking = await _repository.GetByIdAsync(request.Booking.Id, cancellationToken) ?? throw new NotFoundException("Booking not found.");
        
        var stripeSession = !string.IsNullOrEmpty(request.Booking.StripeSessionId) ?
            await _stripeService.GetStripePaymentStatus(
                new GetStripePaymentStatusRequest(request.Booking.StripeSessionId)) :
            null;

        if (stripeSession == null)
            throw new NotFoundException("There is no Stripe Session for this booking.");
        
        var stripeChargeId = stripeSession.PaymentIntent.LatestChargeId;
        var paymentIntentId = stripeSession.PaymentIntentId;

        if (booking.Items.Any(x => x.PropertyId.HasValue))
        {
            foreach (var bookingItem in booking.Items.Where(x => x.PropertyId.HasValue))
            {
                if (string.IsNullOrEmpty(bookingItem.CloudbedsReservationId)) continue;
                
                var property = await _propertyRepository.GetByIdAsync(bookingItem.PropertyId.Value, cancellationToken) ?? throw new Exception("Property not found.");

                await _cloudbedsService.CancelReservation(new CancelReservationRequest(
                    bookingItem.CloudbedsReservationId, property.CloudbedsApiKey, property.CloudbedsPropertyId));
            }
        }
        
        await _repository.DeleteAsync(booking, cancellationToken);
        
        await _stripeService.RefundSession(new RefundSessionRequest(
            stripeChargeId, 
            paymentIntentId,
            request.Booking.TotalAmount));
        
        return true;
    }
}
