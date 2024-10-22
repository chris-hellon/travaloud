using Travaloud.Application.Mailing;

namespace Travaloud.Tenants.SharedRCL.Models.EmailTemplates;

public class TourEnquiryTemplateModel : EmailTemplateBaseModel
{
    public string Name { get; init; } = default!;
    public string Tour { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string ContactNumber { get; init; } = default!;
    public DateTime Date { get; init; }
    public int NumberOfPeople { get; init; }
    public string AdditionalInformation { get; init; } = default!;

    public TourEnquiryTemplateModel()
    {

    }

    public TourEnquiryTemplateModel(string tenantName, string? primaryBackgroundColor, string? secondaryBackgroundColor, string? headerBackgroundColor, string? textColor, string? logoImageUrl, string name, string tour, string email, string contactNumber, DateTime date, int numberOfPeople, string additionalInformation) : base(tenantName, primaryBackgroundColor, secondaryBackgroundColor, headerBackgroundColor, textColor, logoImageUrl)
    {
        Name = name;
        Tour = tour;
        Email = email;
        ContactNumber = contactNumber;
        Date = date;
        NumberOfPeople = numberOfPeople;
        AdditionalInformation = additionalInformation;
    }
}