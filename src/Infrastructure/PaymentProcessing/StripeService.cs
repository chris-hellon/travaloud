using MediatR;
using Stripe;
using Stripe.Checkout;
using Travaloud.Application.PaymentProcessing;
using Travaloud.Application.PaymentProcessing.Commands;
using Travaloud.Application.PaymentProcessing.Queries;

namespace Travaloud.Infrastructure.PaymentProcessing;

public class StripeService : IStripeService
{
    private readonly IMediator _mediator;

    public StripeService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<Session> CreateStripeSession(CreateStripeSessionRequest request)
    {
        return _mediator.Send(request);
    }

    public Task<Session?> GetStripePaymentStatus(GetStripePaymentStatusRequest request)
    {
        return _mediator.Send(request);
    }

    public Task<bool> RefundSession(RefundSessionRequest request)
    {
        return _mediator.Send(request);
    }

    public Task<Customer?> CreateStripeCustomer(CreateStripeCustomerRequest request)
    {
        return _mediator.Send(request);
    }

    public Task<Customer?> SearchStripeCustomer(SearchStripeCustomerRequest request)
    {
        return _mediator.Send(request);
    }

    public Task<Session> CreateStripeSessionClientSecret(CreateStripeSessionClientSecretRequest request)
    {
        return _mediator.Send(request);
    }

    public Task<string> CreateStripeQrCodeUrl(CreateStripeQrCodeRequest request)
    {
        return _mediator.Send(request);
    }
}
