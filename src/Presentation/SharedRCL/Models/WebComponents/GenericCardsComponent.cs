namespace Travaloud.Tenants.SharedRCL.Models.WebComponents;

public class GenericCardsComponent
{
    public string? Title { get; set; }

    public bool FullWidthTitle { get; set; } = false;

    public List<CardComponent> Cards { get; set; }

    public GenericCardsComponent()
    {

    }

    public GenericCardsComponent(string? title, List<CardComponent> cards, bool fullWidthTitle = false)
    {
        Title = title;
        Cards = cards;
        FullWidthTitle = fullWidthTitle;
    }
}