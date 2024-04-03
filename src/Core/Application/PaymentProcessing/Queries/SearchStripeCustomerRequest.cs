using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using Stripe;

namespace Travaloud.Application.PaymentProcessing.Queries;

public class SearchStripeCustomerRequest : IRequest<Customer?>
{
    [Required] public string Email { get; set; }

    public SearchStripeCustomerRequest(string email)
    {
        Email = email;
    }
}

internal class SearchStripeCustomerRequestHandler : IRequestHandler<SearchStripeCustomerRequest, Customer?>
{
    private readonly IStripeClient _stripeClient;

    public SearchStripeCustomerRequestHandler(IOptions<StripeSettings> stripeSettings)
    {
        _stripeClient = new StripeClient(stripeSettings.Value.ApiSecretKey);
    }

    public async Task<Customer?> Handle(SearchStripeCustomerRequest request, CancellationToken cancellationToken)
    {
        var customerSearchOptions = new CustomerSearchOptions
        {
            Query = $"email:'{request.Email}'",
        };

        var customerService = new CustomerService(_stripeClient);
        var matchedCustomers =
            await customerService.SearchAsync(customerSearchOptions, cancellationToken: cancellationToken);

        if (matchedCustomers != null && matchedCustomers.Data.Count != 0)
        {
            return matchedCustomers.Data.First();
        }

        return null;
    }
}