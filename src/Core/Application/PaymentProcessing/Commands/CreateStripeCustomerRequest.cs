using Microsoft.Extensions.Options;
using Stripe;

namespace Travaloud.Application.PaymentProcessing.Commands;

public class CreateStripeCustomerRequest : IRequest<Customer?>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string UserId { get; set; }

    public CreateStripeCustomerRequest(string name, string email, string userId)
    {
        Name = name;
        Email = email;
        UserId = userId;
    }
}

internal class CreateStripeCustomerRequestHandler : IRequestHandler<CreateStripeCustomerRequest, Customer?>
{
    private readonly IStripeClient _stripeClient;

    public CreateStripeCustomerRequestHandler(IOptions<StripeSettings> stripeSettings)
    {
        _stripeClient = new StripeClient(stripeSettings.Value.ApiSecretKey);
    }

    public async Task<Customer?> Handle(CreateStripeCustomerRequest request, CancellationToken cancellationToken)
    {
        var options = new CustomerCreateOptions
        {
            Email = request.Email,
            Name = request.Name,
            Metadata = new Dictionary<string, string>()
            {
                { "Id", request.UserId }
            }
        };
        var service = new CustomerService(_stripeClient);
        return await service.CreateAsync(options, cancellationToken: cancellationToken);
    }
}