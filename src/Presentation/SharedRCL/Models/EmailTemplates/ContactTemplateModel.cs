namespace Travaloud.Tenants.SharedRCL.Models.EmailTemplates;

public class ContactTemplateModel : EmailTemplateBaseModel
{
    public string Name { get; init; }
    public string Email { get; init; }
    public string ContactNumber { get; init; }
    public string Message { get; init; }

    public ContactTemplateModel()
    {

    }

    public ContactTemplateModel(string tenantName,
        string? primaryBackgroundColor,
        string? secondaryBackgroundColor,
        string? headerBackgroundColor,
        string? textColor,
        string? logoImageUrl,
        string name,
        string email,
        string message,
        string contactNumber) : base(tenantName,
        primaryBackgroundColor,
        secondaryBackgroundColor,
        headerBackgroundColor,
        textColor,
        logoImageUrl)
    {
        Name = name;
        Email = email;
        Message = message;
        ContactNumber = contactNumber;
    }
}