using MudBlazor;

namespace Travaloud.Admin.Theme;

public static class CustomColors
{
    public static readonly List<string> ThemeColors = new()
    {
        Light.Primary,
        Colors.Amber.Default,
        Colors.Blue.Default,
        Colors.BlueGrey.Default,
        Colors.Brown.Default,
        Colors.Cyan.Default,
        Colors.DeepOrange.Default,
        Colors.DeepPurple.Default,
        Colors.Green.Default,
        Colors.Indigo.Default,
        Colors.LightBlue.Default,
        Colors.Orange.Default,
        Colors.Pink.Default,
        Colors.Purple.Default,
        Colors.Red.Default,
        Colors.Teal.Default
    };

    public static class Light
    {
        // public const string Primary = "#ab3dff";
        // public const string Tertiary = "#d1ac00";
        // public const string Secondary = "#f66";
        public const string Primary = "#090511";
        public const string Secondary = "#ab3dff";
        public const string Tertiary = "#d1ac00";
        public const string Background = "#F1F3F4";
        public const string AppbarBackground = "#FFF";
        public const string AppbarText = "#153838";
        public const string Info = "#f66";
    }

    public static class Dark
    {
        public const string Primary = "#42D9C8";
        public const string Secondary = "#326771";
        public const string Tertiary = "#2C8C99";
        public const string Background = "#1b1f22";
        public const string AppbarBackground = "#1b1f22";
        public const string DrawerBackground = "#121212";
        public const string Surface = "#202528";
        public const string Disabled = "#545454";
        public const string Info = "#28464B";
    }
}