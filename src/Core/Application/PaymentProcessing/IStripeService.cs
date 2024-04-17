using Stripe;
using Stripe.Checkout;
using Travaloud.Application.PaymentProcessing.Commands;
using Travaloud.Application.PaymentProcessing.Queries;

namespace Travaloud.Application.PaymentProcessing;

public interface IStripeService : ITransientService
{
    /// <summary>
    /// Creates a Stripe Checkout session.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<Session> CreateStripeSession(CreateStripeSessionRequest request);

    /// <summary>
    /// Gets a payment status for a Checkout Session.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<Session?> GetStripePaymentStatus(GetStripePaymentStatusRequest request);

    /// <summary>
    /// Refunds a Checkout Session.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<bool> RefundSession(RefundSessionRequest request);

    
    /// <summary>
    /// Creates a Stripe Customer.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<Customer?> CreateStripeCustomer(CreateStripeCustomerRequest request);

    /// <summary>
    /// Searches for a Stripe Customer.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<Customer?> SearchStripeCustomer(SearchStripeCustomerRequest request);

    /// <summary>
    /// Creates a Stripe Client Secret for use in stripe.js
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<Session> CreateStripeSessionClientSecret(CreateStripeSessionClientSecretRequest request);

    Task<string> CreateStripeQrCodeUrl(CreateStripeQrCodeRequest request);
}