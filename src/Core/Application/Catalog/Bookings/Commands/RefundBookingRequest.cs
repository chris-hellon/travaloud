using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Bookings.Specification;
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
    public DefaultIdType BookingId { get; set; }
    public decimal Amount { get; set; }
    public bool Delete { get; set; }
    public bool IsPartialRefund { get; set; }

    public RefundBookingRequest(DefaultIdType bookingId, decimal amount, bool delete, bool isPartialRefund = false)
    {
        BookingId = bookingId;
        Delete = delete;
        IsPartialRefund = isPartialRefund;
        Amount = amount;
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
        var booking = await _repository.FirstOrDefaultAsync(new BookingByIdWithDetailsSpec(request.BookingId), cancellationToken) ?? throw new NotFoundException("Booking not found.");
        
        var stripeSession = !string.IsNullOrEmpty(booking.StripeSessionId) ?
            await _stripeService.GetStripePaymentStatus(
                new GetStripePaymentStatusRequest(booking.StripeSessionId)) :
            null;
        
        var updateStripeSession = !string.IsNullOrEmpty(booking.UpdateStripeSessionId) ?
            await _stripeService.GetStripePaymentStatus(
                new GetStripePaymentStatusRequest(booking.UpdateStripeSessionId)) :
            null;

        if (stripeSession == null)
        {
            if (booking.StripeSessionId != null && booking.StripeSessionId.Contains("cs_test") && request.Delete)
                await _repository.DeleteAsync(booking, cancellationToken);
            
            throw new NotFoundException("There is no Stripe Session for this booking.");
        }
        
        var stripeChargeId = stripeSession.PaymentIntent.LatestChargeId;
        var paymentIntentId = stripeSession.PaymentIntentId;

        if (!request.IsPartialRefund)
        {
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
        }
        
        if (request.Delete)
        {
            await _repository.DeleteAsync(booking, cancellationToken);
        }
        else
        {
            if (!request.IsPartialRefund)
            {
                var refundedBooking = booking.FlagBookingAsRefunded();
                await _repository.UpdateAsync(refundedBooking, cancellationToken);
            }
        }

        if (!request.IsPartialRefund)
        {
            await _stripeService.RefundSession(new RefundSessionRequest(
                stripeChargeId, 
                paymentIntentId,
                stripeSession.AmountTotal.ConvertToDollars()));
        
            if (updateStripeSession == null) return true;
        
            stripeChargeId = updateStripeSession.PaymentIntent.LatestChargeId; 
            paymentIntentId = updateStripeSession.PaymentIntentId;
            
            await _stripeService.RefundSession(new RefundSessionRequest(
                stripeChargeId, 
                paymentIntentId,
                updateStripeSession.AmountTotal.ConvertToDollars()));
        }
        else
        {
            await _stripeService.RefundSession(new RefundSessionRequest(
                stripeChargeId, 
                paymentIntentId,
                request.Amount));
        }

        return true;
    }
}
