using AspNetCore.ReCaptcha;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.TourEnquiries.Commands;

namespace Travaloud.Tenants.SharedRCL.Models.PageModels;

public abstract class TourPageModel : ContactBasePageModel<EmailTemplates.TourEnquiryTemplateModel, EnquireNowComponent>
{
    private readonly ITourEnquiriesService _tourEnquiriesService;

    protected TourPageModel(ITourEnquiriesService tourEnquiriesService)
    {
        _tourEnquiriesService = tourEnquiriesService;
    }

    protected TourPageModel()
    {
        
    }

    [BindProperty]
    public EnquireNowComponent EnquireNowComponent { get; set; }

    public override async Task<IActionResult> OnGetAsync(string? tourName = null)
    {
        await base.OnGetDataAsync();

        EnquireNowComponent = null;
        ModelState.Clear();

        return Page();
    }

    public override Task<IActionResult> OnPostAsync([FromServices] IReCaptchaService service, EmailTemplates.TourEnquiryTemplateModel model, EnquireNowComponent formModel)
    {
        SubmitFunction = async () => await _tourEnquiriesService.CreateAsync(new CreateTourEnquiryRequest
        {
            Name = EnquireNowComponent.Name,
            Email = EnquireNowComponent.Email,
            ContactNumber = EnquireNowComponent.ContactNumber,
            NumberOfPeople = EnquireNowComponent.NumberOfPeople.Value,
            RequestedDate = EnquireNowComponent.Date.Value,
            AdditionalInformation = EnquireNowComponent.AdditionalInformation,
            TourId = EnquireNowComponent.TourId,
        });

        return base.OnPostAsync(service,
            new EmailTemplates.TourEnquiryTemplateModel(TenantName,
                MailSettings?.PrimaryBackgroundColor,
                MailSettings?.SecondaryBackgroundColor,
                MailSettings?.HeaderBackgroundColor,
                MailSettings?.TextColor,
                MailSettings?.LogoImageUrl,
                EnquireNowComponent.Name,
                EnquireNowComponent.TourName,
                EnquireNowComponent.Email,
                EnquireNowComponent.ContactNumber,
                EnquireNowComponent.Date.Value,
                EnquireNowComponent.NumberOfPeople.Value,
                EnquireNowComponent.AdditionalInformation),
            EnquireNowComponent);
    }
}