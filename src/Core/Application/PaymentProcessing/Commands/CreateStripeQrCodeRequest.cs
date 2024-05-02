using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Application.PaymentProcessing.Extensions;
using Travaloud.Application.PaymentProcessing.Queries;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.PaymentProcessing.Commands;

public class CreateStripeQrCodeRequest : IRequest<string>
{
    public DefaultIdType BookingId { get; set; }
    public string? GuestEmail { get; set; }
}

internal class CreateStripeQrCodeRequestHandler : IRequestHandler<CreateStripeQrCodeRequest, string>
{
    private readonly IStripeClient _stripeClient;
    private readonly IRepositoryFactory<Booking> _repository;
    private readonly IStringLocalizer<CreateStripeQrCodeRequestHandler> _localizer;
    private readonly IStripeService _stripeService;
    private readonly StripeSettings _stripeSettings;

    public CreateStripeQrCodeRequestHandler(
        IRepositoryFactory<Booking> repository,
        IStringLocalizer<CreateStripeQrCodeRequestHandler> localizer,
        IStripeService stripeService,
        IHostingEnvironment hostingEnvironment, 
        IMultiTenantContextAccessor multiTenantContextAccessor,
        IOptions<MultiTenantStripeSettings> multiTenantStripeOptions)
    {
        _repository = repository;
        _localizer = localizer;
        _stripeService = stripeService;

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
                _stripeSettings = environment.StripeSettings;
                _stripeClient = new StripeClient(_stripeSettings.ApiSecretKey);
            }
            
            break;
        }
    }
    
    public async Task<string> Handle(CreateStripeQrCodeRequest request, CancellationToken cancellationToken)
    {
        var booking = await _repository.FirstOrDefaultAsync(
                          new BookingByIdSpec(request.BookingId), cancellationToken) 
                      ?? throw new NotFoundException(string.Format(_localizer["booking.notfound"], request.BookingId));
        
        var propertiesLineItemsParsed =  booking?.Items.Where(x => x is {PropertyId: not null, Rooms: not null}).GetSessionLineItemOptions(true);
        var toursLineItemsParsed = booking?.Items.Where(x => x is {TourId: not null}).GetSessionLineItemOptions(false);

        var propertiesLineItems = propertiesLineItemsParsed?.Item2;
        var toursLineItems = toursLineItemsParsed?.Item2;
        
        var propertiesLabel = propertiesLineItemsParsed?.Item1;
        var toursLabel = propertiesLineItemsParsed?.Item1;
        
        var lineItems = propertiesLineItems!.Union(toursLineItems!).ToList();

        var description = string.Join(", ", propertiesLabel, toursLabel);
        description = description.Trim().TrimEnd(',');
        
        var options = new SessionCreateOptions
        {
            ClientReferenceId = booking?.Id.ToString(),
            LineItems = lineItems,
            Mode = "payment",
            PaymentIntentData = new SessionPaymentIntentDataOptions()
            {
                ReceiptEmail = request.GuestEmail,
                Metadata = new Dictionary<string, string>
                {
                    {"Properties", propertiesLabel ?? string.Empty},
                    {"Tours", toursLabel ?? string.Empty}
                },
                Description = description
            },
            UiMode = "hosted",
            SuccessUrl = _stripeSettings.QRCodeUrl + "/travaloud-payment-confirmation/{CHECKOUT_SESSION_ID}",
            CancelUrl = _stripeSettings.QRCodeUrl + "/order-failed"
        };
        
        var existingCustomer = await _stripeService.SearchStripeCustomer(new SearchMultiTenantStripeCustomerRequest(request.GuestEmail!));
            
        string? customerId = null;

        if (existingCustomer != null)
        {
            customerId = existingCustomer.Id;
        }

        if (!string.IsNullOrEmpty(customerId))
            options.Customer = customerId;
        else
        {
            options.CustomerEmail = request.GuestEmail;
            options.CustomerCreation = "always";
        }

        var service = new SessionService(_stripeClient);
        var session = await service.CreateAsync(options, cancellationToken: cancellationToken);

        return session.Url;
    }
}