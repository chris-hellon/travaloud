namespace Travaloud.Application.Multitenancy;

public class TenantDto
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? ConnectionString { get; set; }
    public string AdminEmail { get; set; } = default!;
    public bool IsActive { get; set; }
    public DateTime ValidUpto { get; set; }
    public string? Issuer { get; set; }
    public string? Url { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? LogoImageUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public string? PrimaryHoverColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string? SecondaryHoverColor { get; set; }
    public string? TeritaryColor { get; set; }
    public string? TeritaryHoverColor { get; set; }
    public string? HeaderFontWoffUrl { get; set; }
    public string? HeaderFontWoff2Url { get; set; }
    public string? HeaderFont { get; set; }
    public string? BodyFontWoffUrl { get; set; }
    public string? BodyFontWoff2Url { get; set; }
    public string? BodyFont { get; set; }
}