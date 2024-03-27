using Travaloud.Application.Checkout.Commands;

namespace Travaloud.Application.Checkout;

public interface ICheckoutService : ITransientService
{
    Task<string> CreatePaymentLink(CreatePaymentLinkRequest request);
}