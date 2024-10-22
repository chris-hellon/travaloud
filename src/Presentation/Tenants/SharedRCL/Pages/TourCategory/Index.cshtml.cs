using Travaloud.Application.Catalog.PageSorting.Dto;
using Travaloud.Application.Catalog.Tours.Dto;

namespace Travaloud.Tenants.SharedRCL.Pages.TourCategory;

public class IndexModel : TravaloudBasePageModel
{
    public override string MetaKeywords(string? overrideValue = null)
    {
        if (TourCategory == null && !string.IsNullOrEmpty(TourCategory?.MetaKeywords))
            return "budget hostels, cheap hostels, backpackers hostels, Vietnam travel";

        return TourCategory?.MetaKeywords ?? string.Empty;
    }

    public override string MetaDescription(string? overrideValue = null)
    {
        if (TourCategory == null && !string.IsNullOrEmpty(TourCategory?.MetaDescription))
            return "Our budget hostels in Vietnam offer comfortable and affordable accommodation for backpackers and budget travelers. Book now and start your Vietnam adventure!";

        return TourCategory?.MetaDescription ?? string.Empty;
    }

    public override string MetaImageUrl()
    {
        if (TourCategory == null && !string.IsNullOrEmpty(TourCategory?.ImagePath))
            return base.MetaImageUrl();

        return TourCategory?.ImagePath ?? string.Empty;
    }

    [BindProperty]
    public PageCategoryComponent? Cards { get; private set; }

    [BindProperty]
    public TourWithCategoryDto? TourCategory { get; private set; }

    public async Task<IActionResult> OnGetAsync(string? tourCategoryName = null)
    {
        await OnGetDataAsync();

        if (ToursWithCategories == null) return Page();
        
        TourCategory = tourCategoryName != null ? ToursWithCategories.FirstOrDefault(x => x.FriendlyUrl == tourCategoryName && !x.TourCategoryId.HasValue) : null;

        if (TourCategory == null) return Page();
        {
            var toursWithCategories = ToursWithCategories.ToList();

            var pageSortings = PageSortings.Where(x => x.ParentTourCategoryId == TourCategory.Id);

            var pageSortingDtos = pageSortings as PageSortingDto[] ?? pageSortings.ToArray();
            
            if (pageSortingDtos.Length != 0)
            {
                toursWithCategories = toursWithCategories.Select(x =>
                {
                    if (pageSortingDtos.Any(ps => ps.TourCategoryId == x.Id))
                    {
                        x.SortOrder = pageSortingDtos.First(ps => ps.TourCategoryId == x.Id).SortOrder;
                    }
                    else if (pageSortingDtos.Any(ps => ps.TourId == x.Id))
                    {
                        x.SortOrder = pageSortingDtos.First(ps => ps.TourId == x.Id).SortOrder;
                    }

                    return x;
                }).OrderBy(x => x.SortOrder).ToList();
            }

            Cards = WebComponentsBuilder.GetToursWithCategoriesPageCategoryComponent(TenantId, toursWithCategories, TourCategory, false, true);
            var tourCategoryNavLink = NavigationSettings?.NavigationLinks.FirstOrDefault(x => x.ChildrenEntity != null && (x.ChildrenEntity == "Tours" || x.ChildrenEntity == "ToursWithCategories"));

            if (TourCategory != null)
            {
                ViewData["Title"] = TourCategory.Name;
                ViewData["TourInfo"] = TourCategory.Description;
            }
            else if (tourCategoryNavLink != null)
                ViewData["Title"] = tourCategoryNavLink.Title;
        }

        return Page();
    }
}