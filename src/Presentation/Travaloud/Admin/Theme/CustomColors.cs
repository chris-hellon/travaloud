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
        public const string Primary = "#153838";
        public const string Secondary = "#CE400B";
        public const string Tertiary = "#16DADF";
        public const string Background = "#F1F3F4";
        public const string AppbarBackground = "#FFF";
        public const string AppbarText = "#153838";
    }

    public static class Dark
    {
        public const string Primary = "#16DADF";
        public const string Secondary = "#2196f3";
        public const string Background = "#1b1f22";
        public const string AppbarBackground = "#1b1f22";
        public const string DrawerBackground = "#121212";
        public const string Surface = "#202528";
        public const string Disabled = "#545454";
    }
}