namespace Travaloud.Infrastructure.Multitenancy.TenantWebsite;

public class ReCaptchaSettings
{
    public string SiteKey { get; set; } = default!;
    public string SecretKey { get; set; } = default!;
    public string Version { get; set; } = default!;
}