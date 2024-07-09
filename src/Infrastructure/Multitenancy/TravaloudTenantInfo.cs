using Finbuckle.MultiTenant;
using Travaloud.Shared.Multitenancy;

namespace Travaloud.Infrastructure.Multitenancy;

public class TravaloudTenantInfo : ITenantInfo
{
    public TravaloudTenantInfo()
    {
    }

    public TravaloudTenantInfo(string id, string name, string? connectionString, string adminEmail, string? issuer = null, string? url = null, string? websiteUrl = null, string? logoImageUrl = null, string? primaryColor = null, string? primaryHoverColor = null, string? secondaryColor = null, string? secondaryHoverColor = null, string? teritaryColor = null, string? teritaryHoverColor = null, string? headerFontWoffUrl = null, string? headerFontWoff2Url = null, string? headerFont = null, string? bodyFontWoffUrl = null, string? bodyFontWoff2Url = null, string? bodyFont = null)
    {
        Id = id;
        Identifier = id;
        Name = name;
        ConnectionString = connectionString ?? string.Empty;
        AdminEmail = adminEmail;
        IsActive = true;
        Issuer = issuer;
        Url = url;
        WebsiteUrl = websiteUrl;
        LogoImageUrl = logoImageUrl;
        PrimaryColor = primaryColor;
        PrimaryHoverColor = primaryHoverColor;
        SecondaryColor = secondaryColor;
        SecondaryHoverColor = secondaryHoverColor;
        TeritaryColor = teritaryColor;
        TeritaryHoverColor = teritaryHoverColor;
        HeaderFontWoffUrl = headerFontWoffUrl;
        HeaderFontWoff2Url = headerFontWoff2Url;
        HeaderFont = headerFont;
        BodyFontWoffUrl = bodyFontWoffUrl;
        BodyFontWoff2Url = bodyFontWoff2Url;
        BodyFont = bodyFont;

        // Add Default 1 Month Validity for all new tenants. Something like a DEMO period for tenants.
        ValidUpto = DateTime.UtcNow.AddMonths(1);
    }

    public TravaloudTenantInfo Update(string id, string name, string? connectionString, string adminEmail, string? issuer = null, string? url = null, string? websiteUrl = null, string? logoImageUrl = null, string? primaryColor = null, string? primaryHoverColor = null, string? secondaryColor = null, string? secondaryHoverColor = null, string? teritaryColor = null, string? teritaryHoverColor = null, string? headerFontWoffUrl = null, string? headerFontWoff2Url = null, string? headerFont = null, string? bodyFontWoffUrl = null, string? bodyFontWoff2Url = null, string? bodyFont = null)
    {
        Id = id;
        Identifier = id;
        Name = name;
        ConnectionString = connectionString ?? string.Empty;
        AdminEmail = adminEmail;
        IsActive = true;
        Issuer = issuer;
        Url = url;
        WebsiteUrl = websiteUrl;
        LogoImageUrl = logoImageUrl;
        PrimaryColor = primaryColor;
        PrimaryHoverColor = primaryHoverColor;
        SecondaryColor = secondaryColor;
        SecondaryHoverColor = secondaryHoverColor;
        TeritaryColor = teritaryColor;
        TeritaryHoverColor = teritaryHoverColor;
        HeaderFontWoffUrl = headerFontWoffUrl;
        HeaderFontWoff2Url = headerFontWoff2Url;
        HeaderFont = headerFont;
        BodyFontWoffUrl = bodyFontWoffUrl;
        BodyFontWoff2Url = bodyFontWoff2Url;
        BodyFont = bodyFont;

        return this;
    }

    public TravaloudTenantInfo Update(string? name, string? connectionString, string adminEmail, string? issuer, string? url, string? websiteUrl, string? logoImageUrl, string? primaryColor, string? primaryHoverColor, string? secondaryColor, string? secondaryHoverColor, string? teritaryColor, string? teritaryHoverColor, string? headerFontWoffUrl, string? headerFontWoff2Url, string? headerFont, string? bodyFontWoffUrl, string? bodyFontWoff2Url, string? bodyFont)
    {
        if (name is not null && Name?.Equals(name) is not true) Name = name;
        if (connectionString is not null && ConnectionString?.Equals(connectionString) is not true) ConnectionString = connectionString;
        if (adminEmail is not null && AdminEmail?.Equals(adminEmail) is not true) AdminEmail = adminEmail;
        if (issuer is not null && Issuer?.Equals(issuer) is not true) Issuer = issuer;
        if (url is not null && Url?.Equals(url) is not true) Url = url;
        if (websiteUrl is not null && WebsiteUrl?.Equals(websiteUrl) is not true) WebsiteUrl = websiteUrl;
        if (logoImageUrl is not null && LogoImageUrl?.Equals(logoImageUrl) is not true) LogoImageUrl = logoImageUrl;
        if (primaryColor is not null && PrimaryColor?.Equals(primaryColor) is not true) PrimaryColor = primaryColor;
        if (primaryHoverColor is not null && PrimaryHoverColor?.Equals(primaryHoverColor) is not true) PrimaryHoverColor = primaryHoverColor;
        if (secondaryColor is not null && SecondaryColor?.Equals(secondaryColor) is not true) SecondaryColor = secondaryColor;
        if (secondaryHoverColor is not null && SecondaryHoverColor?.Equals(secondaryHoverColor) is not true) SecondaryHoverColor = secondaryHoverColor;
        if (teritaryColor is not null && TeritaryColor?.Equals(teritaryColor) is not true) TeritaryColor = teritaryColor;
        if (teritaryHoverColor is not null && TeritaryHoverColor?.Equals(teritaryHoverColor) is not true) TeritaryHoverColor = teritaryHoverColor;
        if (headerFontWoffUrl is not null && HeaderFontWoffUrl?.Equals(headerFontWoffUrl) is not true) HeaderFontWoffUrl = headerFontWoffUrl;
        if (headerFontWoff2Url is not null && HeaderFontWoff2Url?.Equals(headerFontWoff2Url) is not true) HeaderFontWoff2Url = headerFontWoff2Url;
        if (headerFont is not null && HeaderFont?.Equals(headerFont) is not true) HeaderFont = headerFont;
        if (bodyFontWoffUrl is not null && BodyFontWoffUrl?.Equals(bodyFontWoffUrl) is not true) BodyFontWoffUrl = bodyFontWoffUrl;
        if (bodyFontWoff2Url is not null && BodyFontWoff2Url?.Equals(bodyFontWoff2Url) is not true) BodyFontWoff2Url = bodyFontWoff2Url;
        if (bodyFont is not null && BodyFont?.Equals(bodyFont) is not true) BodyFont = bodyFont;

        return this;
    }

    /// <summary>
    /// The actual TenantId, which is also used in the TenantId shadow property on the multitenant entities.
    /// </summary>
    public string Id { get; set; } = default!;

    /// <summary>
    /// The identifier that is used in headers/routes/querystrings. This is set to the same as Id to avoid confusion.
    /// </summary>
    public string Identifier { get; set; } = default!;

    public string Name { get; set; } = default!;
    public string ConnectionString { get; set; } = default!;

    public string AdminEmail { get; private set; } = default!;
    public bool IsActive { get; private set; }
    public DateTime ValidUpto { get; private set; }

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
    
    /// <summary>
    /// Used by AzureAd Authorization to store the AzureAd Tenant Issuer to map against.
    /// </summary>
    public string? Issuer { get; set; }

    public void AddValidity(int months) =>
        ValidUpto = ValidUpto.AddMonths(months);

    public void SetValidity(in DateTime validTill) =>
        ValidUpto = ValidUpto < validTill
            ? validTill
            : throw new Exception("Subscription cannot be backdated.");

    public void Activate()
    {
        if (Id == MultitenancyConstants.Root.Id)
        {
            throw new InvalidOperationException("Invalid Tenant");
        }

        IsActive = true;
    }

    public void Deactivate()
    {
        if (Id == MultitenancyConstants.Root.Id)
        {
            throw new InvalidOperationException("Invalid Tenant");
        }

        IsActive = false;
    }

    string? ITenantInfo.Id { get => Id; set => Id = value ?? throw new InvalidOperationException("Id can't be null."); }
    string? ITenantInfo.Identifier { get => Identifier; set => Identifier = value ?? throw new InvalidOperationException("Identifier can't be null."); }
    string? ITenantInfo.Name { get => Name; set => Name = value ?? throw new InvalidOperationException("Name can't be null."); }
    string? ITenantInfo.ConnectionString { get => ConnectionString; set => ConnectionString = value ?? throw new InvalidOperationException("ConnectionString can't be null."); }
}