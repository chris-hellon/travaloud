using Microsoft.AspNetCore.Http;
using Stripe.Checkout;
using Travaloud.Application.Basket.Dto;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.PaymentProcessing.Commands;

public class CreateStripeSessionClientSecretRequest : IRequest<Session>
{
    public BasketModel? Basket { get; set; }
    public DefaultIdType? GuestId { get; set; }
}

internal class CreateStripeSessionClientSecretRequesttHandler : IRequestHandler<CreateStripeSessionClientSecretRequest, Session>
{
    private readonly IStripeService _stripeService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateStripeSessionClientSecretRequesttHandler(IStripeService stripeService,
        IHttpContextAccessor httpContextAccessor)
    {
        _stripeService = stripeService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Session> Handle(CreateStripeSessionClientSecretRequest request, CancellationToken cancellationToken)
    {
        if (request.Basket == null || !request.GuestId.HasValue)
            throw new NotFoundException("No basket or guest id provided.");
      
        var url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}";

        var stripeSession = await _stripeService.CreateStripeSession(new CreateStripeSessionRequest(
            url + "/payment-confirmation/{CHECKOUT_SESSION_ID}",
            $"{url}/checkout",
            request.Basket,
            request.Basket.Email,
            "embedded"
        )
        {
            GuestId = request.GuestId
        });
        return stripeSession;
    }
}