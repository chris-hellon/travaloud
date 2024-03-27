using MediatR;
using Travaloud.Application.PaymentProcessing;
using Travaloud.Application.PaymentProcessing.Commands;
using Travaloud.Application.PaymentProcessing.Queries;
using Stripe.Checkout;

namespace Travaloud.Infrastructure.Stripe;

public class StripeService : IStripeService
{
    private readonly IMediator _mediator;

    public StripeService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<Session> CreateStripeSession(CreateStripeSessionRequest request)
    {
        return await _mediator.Send(request);
    }

    public async Task<Session> GetStripePaymentStatus(GetStripePaymentStatusRequest request)
    {
        return await _mediator.Send(request);
    }
}
