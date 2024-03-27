using MediatR;
using Travaloud.Application.Checkout;
using Travaloud.Application.Checkout.Commands;

namespace Travaloud.Infrastructure.Catalog.Services;

public class CheckoutService : BaseService, ICheckoutService
{
    public CheckoutService(ISender mediator) : base(mediator)
    {
    }

    public Task<string> CreatePaymentLink(CreatePaymentLinkRequest request)
    {
        return Mediator.Send(request);
    }
}