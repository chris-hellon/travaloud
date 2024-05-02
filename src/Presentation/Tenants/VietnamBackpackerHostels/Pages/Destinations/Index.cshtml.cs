using Microsoft.AspNetCore.Mvc;

namespace VietnamBackpackerHostels.Pages.Destinations;

public class IndexModel : TravaloudBasePageModel
{
    public override string MetaDescription()
    {
        return "Discover comfortable and affordable backpacker hostels in Vietnam with Vietnam Backpacker Hostels. Choose from a range of locations across the country to find the perfect base for your travels.";
    }

    public override string MetaKeywords()
    {
        return "Vietnam Backpacker Hostels, backpacker hostels, affordable hostels, comfortable hostels, Vietnam travel, backpacker accommodation, budget-friendly hostels.";
    }

    [BindProperty]
    public PageCategoryComponent Cards { get; private set; }

    [BindProperty]
    public BookNowComponent BookNowBanner { get; private set; }


    public async Task<IActionResult> OnGetAsync()
    {
        await base.OnGetDataAsync();

        Cards = WebComponentsBuilder.VietnamBackpackerHostels.GetDestinationsCategoryPage(Destinations);
        BookNowBanner = new BookNowComponent(Properties);

        return Page();
    }
}