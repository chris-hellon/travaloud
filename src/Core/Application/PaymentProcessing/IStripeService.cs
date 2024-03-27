using Stripe.Checkout;
using Travaloud.Application.PaymentProcessing.Commands;
using Travaloud.Application.PaymentProcessing.Queries;

namespace Travaloud.Application.PaymentProcessing;

public interface IStripeService : ITransientService
{
    Task<Session> CreateStripeSession(CreateStripeSessionRequest request);

    Task<Session> GetStripePaymentStatus(GetStripePaymentStatusRequest request);
}