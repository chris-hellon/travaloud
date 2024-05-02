using Microsoft.AspNetCore.Mvc;
using Travaloud.Infrastructure.Common.Extensions;

namespace VietnamBackpackerHostels.Pages.Tour;

public class IndexModel : TourPageModel
{
    [BindProperty]
    public GenericCardsComponent RelatedToursCards { get; private set; }
    
    public IndexModel(ITourEnquiriesService tourEnquiriesService, IToursService toursService) : base(tourEnquiriesService, toursService)
    {
    }

    public override async Task<IActionResult> OnGetAsync(string? tourName = null, Guid? tourDate = null, int? guestQuantity = null, string? userId = null)
    {
        await OnGetTourAsync(tourName, tourDate, guestQuantity);

        if (ToursWithCategories != null && ToursWithCategories.Any())
        {
            var relatedTours = ToursWithCategories.Where(t => t.Id != Tour.Id).ToList();

            if (relatedTours != null)
            {
                relatedTours.Shuffle();
                relatedTours = relatedTours.Take(3).ToList();

                RelatedToursCards = WebComponentsBuilder.GetToursWithCategoriesGenericCards(TenantId, relatedTours, null, true, "More Like This", true);
            }
        }

        //Tour.TourItineraries = await ApplicationRepository.GetTourItinerariesAsync(Tour.Id);

        return Page();
    }
}