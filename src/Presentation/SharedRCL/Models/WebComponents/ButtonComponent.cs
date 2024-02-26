namespace Travaloud.Tenants.SharedRCL.Models.WebComponents;

public class ButtonComponent
{
    public string CssClass { get; private set; }
    public string Href { get; private set; }
    public string ButtonText { get; private set; }

    public ButtonComponent()
    {

    }

    public ButtonComponent(string cssClass = null, string href = null, string buttonText = null)
    {
        CssClass = cssClass ?? "btn-primary";
        Href = href;
        ButtonText = buttonText ?? "FIND OUT MORE";
    }

    public ButtonComponent(string href = null, string buttonText = null)
    {
        CssClass = "btn-primary";
        Href = href;
        ButtonText = buttonText ?? "FIND OUT MORE";
    }
}