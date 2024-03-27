using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Travaloud.Application.PaymentProcessing;

namespace Travaloud.Infrastructure.Stripe;

public static class Startup
{
    public static IServiceCollection AddStripe(this IServiceCollection services, IConfiguration config)=>
        services.Configure<StripeSettings>(config.GetSection(nameof(StripeSettings)));
}