using System.Drawing;

namespace Travaloud.Infrastructure.Multitenancy.TenantWebsite;

public class TravaloudSettings
{
    public TravaloudTenantSettings Tenant { get; set; } = default!;
    public TravaloudUrlSettings? UrlConfiguration { get; set; } 
    public TravaloudMetaDataSettings? MetaData { get; set; }
    public TravaloudThemeSettings Theme { get; set; } = default!;
    public TravaloudNavigationSettings? NavigationConfiguration { get; set; } = default!;
}

public class TravaloudTenantSettings
{
    public string TenantId { get; set; }  = default!;
    public string TenantName { get; set; }  = default!;
    public string TenantTagLine { get; set; } = default!;
    public string GoogleTagManagerKey { get; set; } = default!;
    public string GoogleSiteVerificationKey { get; set; }  = default!;
    public string FacebookPageId { get; set; }  = default!;
    public string GuestRoleId { get; set; }  = default!;
    public TravaloudTenantSocialLinksSettings? SocialLinks { get; set; }
}

public class TravaloudTenantSocialLinksSettings
{
    public string? Facebook { get; set; }
    public string? Instagram { get; set; }
    public string? YouTube { get; set; }
}

public class TravaloudUrlSettings
{
    public string WebsiteUrl { get; set; } = "https://www.travaloud.com";
    public string PropertyBookingUrl { get; set; } = "property-booking";
    public string TourBookingUrl { get; set; } = "tour-booking";
    public string AccountManagementUrl { get; set; } = "my-account/profile";
    public string AccountManagementImageUrl { get; set; } = "https://travaloudcdn.azureedge.net/vbh/assets/images/home-page-banner-1.webp?w=2000";
    public string TourUrl { get; set; } = "explore";
    public string TourCategoryUrl { get; set; } = "explore";
}

public class TravaloudMetaDataSettings
{
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaImageUrl { get; set; }
}

public class TravaloudThemeSettings
{
    public string PrimaryColor { get; set; } = default!;
    public string PrimaryColorRgb { get; set; } = default!;
    public string PrimaryHoverColor { get; set; } = default!;
    public string SecondaryColor { get; set; } = default!;
    public string SecondaryColorRgb { get; set; } = default!;
    public string SecondaryHoverColor { get; set; } = default!;
    public string HeaderFont { get; set; } = "Roboto";
    public string BodyFont { get; set; } = "Roboto";
    public string ButtonFont { get; set; } = "Roboto";
    public string NavigationFont { get; set; } = "Roboto";

    public string GenerateRgba(string backgroundColor, decimal backgroundOpacity)
    {
        var color = ColorTranslator.FromHtml(backgroundColor);
        int r = Convert.ToInt16(color.R);
        int g = Convert.ToInt16(color.G);
        int b = Convert.ToInt16(color.B);
        return $"{r}, {g}, {b}";
    }
}

public class TravaloudNavigationSettings
{
    public NavigationLinkModel[] NavigationLinks { get; set; } = default!;
    public bool FullWidth { get; set; } = false;
    public bool LinksCentreAligned { get; set; } = false;
    public bool LinksRightAligned { get; set; } = false;
    public bool ShowSocialIcons { get; set; } = false;
    public bool ShowBookNowButton { get; set; } = false;
    public string NavBrandLogo { get; set; } = default!;
}