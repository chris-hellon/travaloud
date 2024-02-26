namespace Travaloud.Tenants.SharedRCL.Models.WebComponents;

public class FullImageCardsComponent
{
    public string Title { get; set; }

    public List<CardComponent> Cards { get; set; }

    public FullImageCardsComponent()
    {

    }

    public FullImageCardsComponent(string title, List<CardComponent> cards)
    {
        Title = title;
        Cards = cards;
    }
}