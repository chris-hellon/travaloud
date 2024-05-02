using AspNetCore.ReCaptcha;
using Travaloud.Application.Catalog.Enquiries.Commands;
using Travaloud.Application.Catalog.Interfaces;

namespace Travaloud.Tenants.SharedRCL.Models.PageModels;

public abstract class ContactPageModel : ContactBasePageModel<EmailTemplates.ContactTemplateModel, ContactComponent>
{
    private readonly IGeneralEnquiriesService _generalEnquiriesService;

    protected ContactPageModel(IGeneralEnquiriesService generalEnquiriesService)
    {
        _generalEnquiriesService = generalEnquiriesService;
    }

    [BindProperty]
    public ContactComponent ContactComponent { get; set; }

    public override async Task<IActionResult> OnGetAsync(string? tourName = null, Guid? tourDate = null, int? guestQuantity = null, string? userId = null)
    {
        await base.OnGetDataAsync();

        ContactComponent = new ContactComponent()
        {
            Tours = Tours,
            Properties = Properties
        };
        ModelState.Clear();

        return Page();
    }

    public override Task<IActionResult> OnPostAsync([FromServices] IReCaptchaService service, EmailTemplates.ContactTemplateModel model, ContactComponent formModel)
    {
        SubmitFunction = async () => await _generalEnquiriesService.CreateAsync(new CreateGeneralEnquiryRequest
        {
            Name = ContactComponent.Name,
            Email = ContactComponent.Email,
            ContactNumber = ContactComponent.ContactNumber,
            Message = ContactComponent.Message,
            RelatedProperty = ContactComponent.RelatedProperty,
            RelatedTour = ContactComponent.RelatedTour
        });

        return base.OnPostAsync(service,
            new EmailTemplates.ContactTemplateModel(TenantName,
                MailSettings?.PrimaryBackgroundColor,
                MailSettings?.SecondaryBackgroundColor,
                MailSettings?.HeaderBackgroundColor,
                MailSettings?.TextColor,
                MailSettings?.LogoImageUrl,
                ContactComponent.Name,
                ContactComponent.Email,
                ContactComponent.Message,
                ContactComponent.ContactNumber,
                ContactComponent.RelatedTour,
                ContactComponent.RelatedProperty),
            ContactComponent);
    }
}