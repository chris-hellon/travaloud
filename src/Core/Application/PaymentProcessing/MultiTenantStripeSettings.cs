namespace Travaloud.Application.PaymentProcessing;

public class MultiTenantStripeSettings
{
    public List<TenantStripeSettings> Tenants { get; set; }
}

public class TenantStripeSettings
{
    public string? TenantIdentifier { get; set; }
    public string? TenantName { get; set; }
    public string? TenantUrl { get; set; }
    public string? ToAddress { get; set; }
    public string? PrimaryBackgroundColor { get; set; }
    public string? SecondaryBackgroundColor { get; set; }
    public string? HeaderBackgroundColor { get; set; }
    public string? TextColor { get; set; }
    public string? LogoImageUrl { get; set; }
    public string[]? BccAddress { get; set; }
    
    public List<TenantEnvironmentStripeSettings>? Environments { get; set; }
}

public class TenantEnvironmentStripeSettings
{
    public string? Environment { get; set; }

    public StripeSettings? StripeSettings { get; set; }
}