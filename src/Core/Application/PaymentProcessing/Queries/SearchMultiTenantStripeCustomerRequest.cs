using System.ComponentModel.DataAnnotations;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Stripe;

namespace Travaloud.Application.PaymentProcessing.Queries;

public class SearchMultiTenantStripeCustomerRequest : IRequest<Customer?>
{
    [Required] public string Email { get; set; }
    public SearchMultiTenantStripeCustomerRequest(string email, bool isAdmin = false)
    {
        Email = email;
    }
}

internal class SearchMultiTenantStripeCustomerRequestHandler : IRequestHandler<SearchMultiTenantStripeCustomerRequest, Customer?>
{
    private readonly IStripeClient _stripeClient;
    public SearchMultiTenantStripeCustomerRequestHandler(
        IHostingEnvironment hostingEnvironment, 
        IMultiTenantContextAccessor multiTenantContextAccessor,
        IOptions<MultiTenantStripeSettings> multiTenantStripeOptions)
    {
        var multiTenantStripeSettings = multiTenantStripeOptions.Value;
        var tenantIdentifier = multiTenantContextAccessor?.MultiTenantContext?.TenantInfo?.Identifier;
        var tenantSettings = multiTenantStripeSettings.Tenants.FirstOrDefault(x => x.TenantIdentifier == tenantIdentifier);

        if (tenantSettings == null)
        {
            throw new ArgumentException($"No settings found for tenant '{tenantIdentifier}'.");
        }

        if (tenantSettings.Environments == null) 
            throw new ArgumentException($"No settings found for tenant '{tenantIdentifier}'.");
            
        foreach (var environment in tenantSettings.Environments.Where(environment => environment.Environment == hostingEnvironment.EnvironmentName))
        {
            if (environment.StripeSettings == null)
                throw new ArgumentException($"No settings found for tenant '{tenantIdentifier}'.");
            
            if (environment.StripeSettings != null)
            {
                _stripeClient = new StripeClient(environment.StripeSettings.ApiSecretKey);
            }
            
            break;
        }
    }

    public async Task<Customer?> Handle(SearchMultiTenantStripeCustomerRequest request, CancellationToken cancellationToken)
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