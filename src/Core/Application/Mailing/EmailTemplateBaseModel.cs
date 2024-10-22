namespace Travaloud.Application.Mailing;

public abstract class EmailTemplateBaseModel
{
    public string TenantName { get; set; }
    public string? PrimaryBackgroundColor { get; set; }
    public string? SecondaryBackgroundColor { get; set; }
    public string? HeaderBackgroundColor { get; set; }
    public string? TextColor { get; set; }
    public string? LogoImageUrl { get; set; }

    public EmailTemplateBaseModel()
    {

    }

    protected EmailTemplateBaseModel(string tenantName, string? primaryBackgroundColor, string? secondaryBackgroundColor, string? headerBackgroundColor, string? textColor, string? logoImageUrl)
    {
        TenantName = tenantName;
        PrimaryBackgroundColor = primaryBackgroundColor;
        SecondaryBackgroundColor = secondaryBackgroundColor;
        HeaderBackgroundColor = headerBackgroundColor;
        TextColor = textColor;
        LogoImageUrl = logoImageUrl;
    }
}