using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Travaloud.Application.PaymentProcessing;

namespace Travaloud.Application.Catalog.Bookings.Commands;

public class CreateStaffTourBookingQrCodeUrlRequest : IRequest<string>
{
    public string UserId { get; set; }
    public string TourName { get; set; }

    public CreateStaffTourBookingQrCodeUrlRequest(string userId, string tourName)
    {
        UserId = userId;
        TourName = tourName;
    }
}

internal class CreateStaffTourBookingQrCodeUrlRequestHandler : IRequestHandler<CreateStaffTourBookingQrCodeUrlRequest, string>
{
    private readonly StripeSettings _stripeSettings;
    
    public CreateStaffTourBookingQrCodeUrlRequestHandler(
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
                _stripeSettings = environment.StripeSettings;
            }
            
            break;
        }
    }

    public Task<string> Handle(CreateStaffTourBookingQrCodeUrlRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(
            $"{_stripeSettings.QRCodeUrl}/tours/staff-booking/{request.TourName.UrlFriendly()}/{request.UserId}");
    }
}