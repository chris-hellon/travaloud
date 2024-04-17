using AspNetCore.ReCaptcha;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.JobVacanciesResponses.Commands;

namespace Travaloud.Tenants.SharedRCL.Models.PageModels;

public class JoinOurCrewPageModel : ContactBasePageModel<EmailTemplates.JoinOurCrewTemplateModel, JoinOurCrewComponent>
{
    private readonly IJobVacancyResponsesService _jobVacancyResponsesService;

    public JoinOurCrewPageModel(IJobVacancyResponsesService jobVacancyResponsesService)
    {
        _jobVacancyResponsesService = jobVacancyResponsesService;
        JoinOurCrewComponent = new JoinOurCrewComponent();
    }

    [BindProperty]
    public JoinOurCrewComponent JoinOurCrewComponent { get; set; }
    
    public override string Subject()
    {
        return "Job Application";
    }

    public override string TemplateName()
    {
        return "JoinOurCrewTemplate";
    }
    
    public override async Task<IActionResult> OnGetAsync(string? tourName = null, Guid? tourDate = null, int? guestQuantity = null)
    {
        await base.OnGetDataAsync();

        JoinOurCrewComponent = new JoinOurCrewComponent();
        ModelState.Clear();

        return Page();
    }

    public override Task<IActionResult> OnPostAsync([FromServices] IReCaptchaService service, EmailTemplates.JoinOurCrewTemplateModel model, JoinOurCrewComponent formModel)
    {
        SubmitFunction = async () => await _jobVacancyResponsesService.CreateAsync(new CreateJobVacancyResponseRequest()
        {
            FirstName = JoinOurCrewComponent.FirstName,
            LastName = JoinOurCrewComponent.LastName,
            HowCanWeCollaborate = JoinOurCrewComponent.HowCanWeCollaborate,
            EstimatedDates = JoinOurCrewComponent.EstimatedDates,
            DestinationsVisited = JoinOurCrewComponent.DestinationsVisited,
            Email = JoinOurCrewComponent.Email,
            Equipment = JoinOurCrewComponent.Equipment,
            Instagram = JoinOurCrewComponent.Instagram,
            JobVacancyId = JoinOurCrewComponent.JobVacancyId,
            Portfolio = JoinOurCrewComponent.Portfolio,
            YouTube = JoinOurCrewComponent.YouTube,
            TikTok = JoinOurCrewComponent.TikTok
        });

        return base.OnPostAsync(service,
            new EmailTemplates.JoinOurCrewTemplateModel(TenantName,
                MailSettings?.PrimaryBackgroundColor,
                MailSettings?.SecondaryBackgroundColor,
                MailSettings?.HeaderBackgroundColor,
                MailSettings?.TextColor,
                MailSettings?.LogoImageUrl, 
            JoinOurCrewComponent.FirstName, 
            JoinOurCrewComponent.LastName, 
            JoinOurCrewComponent.Email, 
            JoinOurCrewComponent.HowCanWeCollaborate,
            JoinOurCrewComponent.EstimatedDates,
            JoinOurCrewComponent.DestinationsVisited,
            JoinOurCrewComponent.Instagram,
            JoinOurCrewComponent.TikTok,
            JoinOurCrewComponent.YouTube,
            JoinOurCrewComponent.Portfolio,
            JoinOurCrewComponent.Equipment,
            JoinOurCrewComponent.JobVacancy,
            JoinOurCrewComponent.Location), JoinOurCrewComponent);
    }
}