using Microsoft.AspNetCore.Mvc;

namespace VietnamBackpackerHostels.Pages.Explore;

public class IndexModel : TravaloudBasePageModel
{
    public override string MetaKeywords(string? overrideValue = null)
    {
        return "Vietnam Backpacker Hostels, explore Vietnam, backpacker travel, Vietnam tours, adventure travel, backpacker-friendly, Vietnam destinations";
    }

    public override string MetaDescription(string? overrideValue = null)
    {
        return "Embark on an adventure of a lifetime with Vietnam Backpacker Hostels. Explore Vietnam's vibrant cities and stunning landscapes with our exciting tours and travel options.";
    }

    [BindProperty]
    public PageCategoryComponent Cards { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        await base.OnGetDataAsync();
        Cards = WebComponentsBuilder.GetToursWithCategoriesPageCategoryComponent(TenantId, ToursWithCategories.Where(x => !x.GroupParentCategoryId.HasValue));

        ViewData["Title"] = "Explore";

        return Page();
    }
}