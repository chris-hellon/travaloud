namespace Travaloud.Shared.Common.Extensions;

public static class MudTextEditorExtensions
{
    public static string? FormatTextEditorString(this string? value)
    {
        return value == "<p><br></p>" ? string.Empty : value?.Replace("<p><br></p>", "");
    }
}