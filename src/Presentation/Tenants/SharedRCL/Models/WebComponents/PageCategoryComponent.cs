namespace Travaloud.Tenants.SharedRCL.Models.WebComponents;

public class PageCategoryComponent
{
	public string Title { get; private set; }
	public string SubTitle { get; private set; }
	public GenericCardsComponent Cards { get; private set; }
	public bool IncludeTextureBanner { get; set; } = true;

	public PageCategoryComponent()
	{
	}

	public PageCategoryComponent(string title, string subTitle, GenericCardsComponent cards, bool includeTextureBanner = true)
	{
		Title = title;
		SubTitle = subTitle;
		Cards = cards;
		IncludeTextureBanner = includeTextureBanner;
	}
}