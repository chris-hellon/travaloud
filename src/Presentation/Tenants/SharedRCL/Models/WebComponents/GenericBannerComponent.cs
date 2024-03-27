namespace Travaloud.Tenants.SharedRCL.Models.WebComponents;

public class GenericBannerComponent
{
    public string Title { get; private set; }

    public string Body { get; private set; }

    public IEnumerable<OvalContainerComponent>? OvalContainers { get; private set; }

    public GenericBannerComponent()
    {

    }

    public GenericBannerComponent(string title, string body, IEnumerable<OvalContainerComponent>? ovalContainers = null)
    {
        Title = title;
        Body = body;
        OvalContainers = ovalContainers;
    }
}