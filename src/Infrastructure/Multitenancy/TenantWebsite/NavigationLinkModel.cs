namespace Travaloud.Infrastructure.Multitenancy.TenantWebsite;

public class NavigationLinkModel
{
    public string Title { get; set; } = default!;
    public string Page { get; set; }= default!;
    public string FriendlyUrl { get; set; }= default!;
    public Dictionary<string, string>? Routes { get; set; } 
    public string? ChildrenEntity { get; set; }
    public NavigationLinkModel[]? Children { get; set; }

    public NavigationLinkModel()
    {
        
    }
    
    public NavigationLinkModel(string title, string page, string friendlyUrl, Dictionary<string, string>? routes = null, string? childrenEntity = null, NavigationLinkModel[]? children = null)
    {
        Title = title;
        Page = page;
        FriendlyUrl = friendlyUrl;
        ChildrenEntity = childrenEntity;
        Children = children;
        Routes = routes;
    }
}