namespace Travaloud.Tenants.SharedRCL.Models.WebComponents;

public class ButtonComponent
{
    public string CssClass { get; private set; } = string.Empty;
    public string Href { get; private set; } = string.Empty;
    public string ButtonText { get; private set; } = string.Empty;

    public ButtonComponent()
    {

    }

    public ButtonComponent(string? cssClass = null, string? href = null, string? buttonText = null)
    {
        CssClass = cssClass ?? "btn-primary";
        Href = href;
        ButtonText = buttonText ?? "FIND OUT MORE";
    }

    public ButtonComponent(string? href = null, string? buttonText = null)
    {
        CssClass = "btn-primary";
        Href = href;
        ButtonText = buttonText ?? "FIND OUT MORE";
    }
}