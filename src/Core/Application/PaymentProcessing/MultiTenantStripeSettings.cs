namespace Travaloud.Application.PaymentProcessing;

public class MultiTenantStripeSettings
{
    public List<TenantStripeSettings> Tenants { get; set; }
}

public class TenantStripeSettings
{
    public string? TenantIdentifier { get; set; }
    public List<TenantEnvironmentStripeSettings>? Environments { get; set; }
}

public class TenantEnvironmentStripeSettings
{
    public string? Environment { get; set; }

    public StripeSettings? StripeSettings { get; set; }
}