using MediatR;
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

    public async Task<Session> CreateStripeSession(CreateStripeSessionRequest request)
    {
        return await _mediator.Send(request);
    }

    public async Task<Session> GetStripePaymentStatus(GetStripePaymentStatusRequest request)
    {
        return await _mediator.Send(request);
    }
}
