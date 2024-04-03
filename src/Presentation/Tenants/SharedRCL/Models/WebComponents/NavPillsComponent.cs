namespace Travaloud.Tenants.SharedRCL.Models.WebComponents;

public class NavPillsComponent
{
    public string Title { get; private set; } = string.Empty;
    public List<NavPill> NavPills { get; private set; } = default!;

    public NavPillsComponent()
    {

    }

    public NavPillsComponent(string title, List<NavPill> navPills)
    {
        Title = title;
        NavPills = navPills;
    }
}

public class NavPill
{
    public string Title { get; private set; } = string.Empty;
    public string FriendlyTitle { get; private set; } = string.Empty;
    public string IdTitle { get; private set; } = string.Empty;
    public decimal AnimationDelay { get; private set; }
    public List<NavPillContent>? Content { get; private set; }

    public NavPill()
    {

    }

    public NavPill(string title, List<NavPillContent>? content = null)
    {
        Title = title;
        FriendlyTitle = title.UrlFriendly();
        IdTitle = title.ConvertToCamelCase();
        Content = content;
    }
    public NavPill(string title, decimal animationDelay, List<NavPillContent>? content = null)
    {
        Title = title;
        FriendlyTitle = title.UrlFriendly();
        IdTitle = title.ConvertToCamelCase();
        AnimationDelay = animationDelay;
        Content = content;
    }
}

public class NavPillContent
{
    public string Body { get; private set; } = string.Empty;
    public string Style { get; private set; } = string.Empty;

    public NavPillContent()
    {

    }

    public NavPillContent(string body, string? style = null)
    {
        Body = body;
        Style = style ?? "";
    }
}