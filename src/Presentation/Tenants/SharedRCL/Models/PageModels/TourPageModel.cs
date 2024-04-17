using System.ComponentModel.DataAnnotations;
using AspNetCore.ReCaptcha;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.TourEnquiries.Commands;
using Travaloud.Application.Catalog.Tours.Dto;

namespace Travaloud.Tenants.SharedRCL.Models.PageModels;

public abstract class TourPageModel : ContactBasePageModel<EmailTemplates.TourEnquiryTemplateModel, EnquireNowComponent>
{
    private readonly ITourEnquiriesService _tourEnquiriesService;
    private readonly IToursService _toursService;
    
    public override string Subject()
    {
        return "Tour Booking Enquiry";
    }

    public override string TemplateName()
    {
        return "TourEnquiryTemplate";
    }

    public override string MetaKeywords()
    {
        return Tour?.MetaKeywords ?? base.MetaKeywords();
    }

    public override string MetaDescription()
    {
        return Tour?.MetaDescription ?? base.MetaDescription();
    }

    public override string MetaImageUrl()
    {
        return Tour?.ImagePath ?? base.MetaImageUrl();
    }

    [BindProperty]
    public TourDetailsDto? Tour { get; set; }

    [BindProperty]
    public GenericCardsComponent? RelatedToursCards { get; private set; }

    [BindProperty]
    public string PageTitle { get; private set; }

    [BindProperty]
    public HeaderBannerComponent HeaderBanner { get; set; }

    [BindProperty]
    public BookNowComponent? BookNowComponent { get; private set; }

    [Required]
    [Display(Name = "Select Date")]
    [DataType(DataType.Date)]
    public DateTime? TourDate { get; set; }
    
    [Required]
    [Display(Name = "Select Time")]
    public TimeSpan? TourDateStartTime { get; set; }
    
    [Required]
    [Display(Name = "No. of Guests")]
    public int? GuestQuantity { get; set; }
    
    public TourDateDto? SelectedTourDate { get; set; }
    
    protected TourPageModel(ITourEnquiriesService tourEnquiriesService, IToursService toursService)
    {
        _tourEnquiriesService = tourEnquiriesService;
        _toursService = toursService;
    }

    [BindProperty]
    public EnquireNowComponent EnquireNowComponent { get; set; }

    public async Task<IActionResult> OnGetTourAsync(string? tourName = null, Guid? tourDate = null, int? guestQuantity = null)
    {
        await base.OnGetDataAsync();

        EnquireNowComponent = new EnquireNowComponent();
        ModelState.Clear();

        var tour = Tours?.FirstOrDefault(x => x.FriendlyUrl == tourName);

        if (tour == null) return Page();
        {
            Tour = await _toursService.GetAsync(tour.Id);

            if (Tour == null) return Page();

            ViewData["Title"] = Tour?.Name;

            EnquireNowComponent = new EnquireNowComponent()
            {
                TourName = Tour.Name,
                TourId = Tour.Id
            };

            BookNowComponent = new BookNowComponent(Tours, Tour.Id);

            if (!tourDate.HasValue || Tour.TourDates == null || !Tour.TourDates.Any()) return Page();
            
            var matchedTourDate = Tour.TourDates.FirstOrDefault(x => x.Id == tourDate.Value);

            if (matchedTourDate == null) return Page();
            
            TourDate = matchedTourDate.StartDate.Date;
            TourDateStartTime = matchedTourDate.StartDate.TimeOfDay;
            GuestQuantity = guestQuantity;
            SelectedTourDate = matchedTourDate;
        }

        return Page();
    }

    public override Task<IActionResult> OnPostAsync([FromServices] IReCaptchaService service, EmailTemplates.TourEnquiryTemplateModel model, EnquireNowComponent formModel)
    {
        SubmitFunction = async () => await _tourEnquiriesService.CreateAsync(new CreateTourEnquiryRequest
        {
            Name = EnquireNowComponent.Name,
            Email = EnquireNowComponent.Email,
            ContactNumber = EnquireNowComponent.ContactNumber,
            NumberOfPeople = EnquireNowComponent.NumberOfPeople!.Value,
            RequestedDate = EnquireNowComponent.Date!.Value,
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
                EnquireNowComponent.Date!.Value,
                EnquireNowComponent.NumberOfPeople!.Value,
                EnquireNowComponent.AdditionalInformation),
            EnquireNowComponent);
    }
}