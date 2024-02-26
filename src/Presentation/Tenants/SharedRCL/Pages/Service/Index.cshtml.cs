using Microsoft.AspNetCore.Http.Extensions;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Services.Dto;
using Travaloud.Application.Catalog.ServicesEnquiries.Commands;
using Travaloud.Application.Common.Mailing;

namespace Travaloud.Tenants.SharedRCL.Pages.Service;

public class IndexModel : TravaloudBasePageModel
{
    private readonly IServicesService _servicesService;
    private readonly IServicesEnquiriesService _servicesEnquiriesService;
    
    public IndexModel(IServicesService servicesService, IServicesEnquiriesService servicesEnquiriesService)
    {
        _servicesService = servicesService;
        _servicesEnquiriesService = servicesEnquiriesService;
    }
    
    [BindProperty]
    public ServiceDetailsDto? Service { get; set; }

    [BindProperty]
    public GenericCardsComponent ServicesCards { get; set; }

    [BindProperty]
    public string ServiceTitle { get; set; }

    [BindProperty]
    public Guid ServiceId { get; set; }

    public async Task<IActionResult> OnGet(string serviceName)
    {
        await base.OnGetDataAsync();

        Service = await _servicesService.GetByNameAsync(serviceName);

        if (Service != null)
        {
            ServiceTitle = Service.Title;
            ServiceId = Service.Id;
        }

        if (Services != null)
            ServicesCards = WebComponentsBuilder.UncutTravel.GetServicesGenericCards(Services.Where(x => x.Id != Service.Id), null);

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        Service.Title = ServiceTitle;

        var emailTemplateModel = new Models.EmailTemplates.ServiceTemplateModel(TenantName,
            MailSettings?.PrimaryBackgroundColor,
            MailSettings?.SecondaryBackgroundColor,
            MailSettings?.HeaderBackgroundColor,
            MailSettings?.TextColor,
            MailSettings?.LogoImageUrl,
            Service);
        var emailHtml = await RazorPartialToStringRenderer.RenderPartialToStringAsync($"/Pages/EmailTemplates/ServiceTemplate.cshtml", emailTemplateModel);

        if (MailSettings?.ToAddress != null)
        {
            var mailRequest = new MailRequest(
                to: [MailSettings.ToAddress],
                subject:$"New {ServiceTitle} Request",
                body: emailHtml,
                bcc: [MailSettings.BccAddress.ToString()]);
        
            await MailService.SendAsync(mailRequest);
        }

        StatusMessage = "<p>Your request has been sent successfully.</p><p>A member of our team will contact you shortly.</p>";

        var fields = emailTemplateModel.Service.ServiceFields.Select(field => new CreateServiceEnquiryFieldRequest()
            {
                Field = field.Label,
                Value = field.Value
            })
            .ToList();

        await _servicesEnquiriesService.CreateAsync(new CreateServiceEnquiryRequest()
        {
            ServiceId = ServiceId,
            Fields = fields
        });

        ModelState.Clear();

        return Redirect(Request.GetEncodedUrl());
    }
}