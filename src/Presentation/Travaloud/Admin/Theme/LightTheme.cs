using MudBlazor;
using Travaloud.Infrastructure.Multitenancy;

namespace Travaloud.Admin.Theme;

public class LightTheme : MudTheme
{
    public LightTheme(TravaloudTenantInfo tenantInfo)
    {
        Palette = new PaletteLight()
        {
            Primary = tenantInfo.PrimaryColor ?? CustomColors.Light.Primary,
            Secondary = tenantInfo.SecondaryColor ?? CustomColors.Light.Secondary,
            Tertiary = tenantInfo.TeritaryColor ?? CustomColors.Light.Tertiary,
            Info = tenantInfo.TeritaryHoverColor ?? CustomColors.Light.Info,
            Background = CustomColors.Light.Background,
            AppbarBackground = CustomColors.Light.AppbarBackground,
            AppbarText = CustomColors.Light.AppbarText,
            DrawerBackground = CustomColors.Light.AppbarBackground,
            DrawerText = "rgba(0,0,0, 0.7)",
            Success = tenantInfo.SecondaryColor ?? CustomColors.Light.Secondary,
            TableLines = "#e0e0e029",
            OverlayDark = "hsl(0deg 0% 0% / 75%)"
        };
        LayoutProperties = new LayoutProperties()
        {
            DefaultBorderRadius = "5px"
        };

        Typography = CustomTypography.TravaloudTypography;
        Shadows = new Shadow();
        ZIndex = new ZIndex() { Drawer = 1300 };
    }
}