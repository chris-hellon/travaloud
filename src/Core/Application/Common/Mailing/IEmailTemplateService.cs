namespace Travaloud.Application.Common.Mailing;

public interface IEmailTemplateService : ITransientService
{
    string GenerateEmailTemplate<T>(string templateName, T mailTemplateModel);

    string GenerateTourManifestEmail(string tourName, string primaryBackgroundColor, string logoImageUrl,
        DateTime startDate, int guestCount, int paidGuestCount, int unPaidGuestCount, string tenantName);
}