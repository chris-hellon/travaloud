using MudBlazor;

namespace Travaloud.Admin.Theme;

public class LightTheme : MudTheme
{
    public LightTheme()
    {
        Palette = new PaletteLight()
        {
            Primary = CustomColors.Light.Primary,
            Secondary = CustomColors.Light.Secondary,
            Tertiary = CustomColors.Light.Tertiary,
            Background = CustomColors.Light.Background,
            AppbarBackground = CustomColors.Light.AppbarBackground,
            AppbarText = CustomColors.Light.AppbarText,
            DrawerBackground = CustomColors.Light.AppbarBackground,
            DrawerText = "rgba(0,0,0, 0.7)",
            Success = CustomColors.Light.Primary,
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