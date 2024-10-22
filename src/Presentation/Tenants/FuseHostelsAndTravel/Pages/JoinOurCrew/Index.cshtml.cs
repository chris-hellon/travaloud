using Microsoft.AspNetCore.Mvc;
using Travaloud.Application.Catalog.JobVacancies.Dto;
using Travaloud.Application.Catalog.JobVacancies.Queries;

namespace FuseHostelsAndTravel.Pages.JoinOurCrew;

public class IndexModel : JoinOurCrewPageModel
{
    private readonly IJobVacanciesService _jobVacanciesService;

    public IndexModel(IJobVacanciesService jobVacanciesService, IJobVacancyResponsesService jobVacancyResponsesService)
        : base(jobVacancyResponsesService)
    {
        _jobVacanciesService = jobVacanciesService;
    }
    
    public override Guid? PageId()
    {
        return new Guid("80d6e158-574c-460d-b52d-1ebf37e05841");
    }

    public override string MetaKeywords(string? overrideValue = null)
    {
        return base.MetaKeywords( "Fuse Hostels & Travel, job opportunities, hospitality industry, travel industry, Vietnam jobs, backpacker jobs");
    }

    public override string MetaDescription(string? overrideValue = null)
    {
        return base.MetaDescription("Join the Fuse Hostels & Travel crew and become a part of our team. We offer exciting job opportunities in the hospitality and travel industry in Vietnam.");
    }
    
    [BindProperty] public HeaderBannerComponent? HeaderBanner { get; private set; }

    [BindProperty] public List<NavPill>? NavPills { get; private set; }

    [BindProperty] public IEnumerable<JobVacancyDto>? JobVacancies { get; set; }

    [BindProperty] public JobVacancyDto? JobVacancy { get; set; }

    [BindProperty] public ContainerHalfImageRoundedTextComponent? IntroductionBanner { get; private set; }

    [BindProperty] public List<CardComponent>? JobVacanciesCards { get; private set; }

    public override async Task<IActionResult> OnGetAsync(string? tourName = null, Guid? tourDate = null, int? guestQuantity = null, string? userId = null)
    {
        await base.OnGetDataAsync();

        ViewData["Title"] = "Join Our Crew";

        var jobVacancies = await _jobVacanciesService.SearchAsync(new SearchJobVacanciesRequest()
        {
            PageSize = 99999,
            PageNumber = 1
        });

        JobVacancies = jobVacancies.Data;

        if (tourName != null && JobVacancies != null)
        {
            var jobVacancyId = Guid.Parse(tourName);
            JobVacancy = JobVacancies?.FirstOrDefault(x => x.Id == jobVacancyId);
            if (JobVacancy != null)
                JoinOurCrewComponent = new JoinOurCrewComponent()
                {
                    JobVacancyId = JobVacancy.Id,
                    JobVacancy = JobVacancy.JobTitle,
                    Location = JobVacancy.Location
                };

            return Page();
        }

        var jobVacancyDtos = JobVacancies as JobVacancyDto[] ?? JobVacancies?.ToArray();
        if (jobVacancyDtos != null && jobVacancyDtos.Length != 0)
        {
            JobVacanciesCards = jobVacancyDtos.Select(x => new CardComponent(title: x.JobTitle, body: x.Location)
            {
                LgClass = 4,
                ButtonComponent = new ButtonComponent("btn-primary", $"/JoinOurCrew/Index?tourName={x.Id}", "Apply Now")
            }).ToList();
        }

        NavPills =
        [
            new NavPill("WHY WORK WITH US", 1400),
            new NavPill("AVAILABLE POSITIONS", 1600),
            new NavPill("STAY UP TO DATE", 1800),
            new NavPill("HOSTELS", 2000)
        ];

        HeaderBanner = new HeaderBannerComponent("JOIN OUR CREW", null, null,
            "https://travaloudcdn.azureedge.net/fuse/assets/images/72067649-43d3-4961-93c7-58e37399860f.jpg?w=2000",
            new List<OvalContainerComponent>()
            {
                new("aboutPageHeaderBannerOvals1", 15, null, -30, null)
            });

        IntroductionBanner = new ContainerHalfImageRoundedTextComponent(new List<string>() {"WHY WORK WITH US"}, null,
                "<p>Ready to make waves and join a crew that's all about fun, adventure, and unforgettable experiences? Look no further! At Fuse Hostels and Travel, we're on the lookout for passionate individuals to join our dynamic team.</p> <p>From hosting epic pool parties to leading exciting tours, there's never a dull moment with us. Whether you're seeking short-term gigs or a more permanent position, we offer competitive benefits and perks to sweeten the deal.</p> <p>So, if you're outgoing, sociable, and ready to deliver top-notch customer service, come aboard and be part of something extraordinary! Explore our current job postings below and take the first step towards an incredible journey with Fuse Hostels and Travel.</p>",
                "https://travaloudcdn.azureedge.net/fuse/assets/images/fuse-purple-logo.webp?w=800", null,
                new List<OvalContainerComponent>()
                {
                    new("aboutPageIntroductionOvals1", 15, null, null, -28),
                    new("aboutPageIntroductionOvals2", null, 15, null, 18)
                })
            {AnimationStart = "onLoad"};

        return Page();
    }
}