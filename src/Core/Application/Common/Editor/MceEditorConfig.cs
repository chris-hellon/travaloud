namespace Travaloud.Application.Common.Editor;

public static class TinyMceConfig
{
    public static Dictionary<string, object> Styles(int height = 400) =>
        new()
        {
            { "content_style", "@import url('https://fonts.googleapis.com/css2?family=Bebas+Neue&family=Montserrat:ital,wght@0,100..900;1,100..900&family=Playfair+Display:ital,wght@0,400..900;1,400..900&family=Roboto:ital,wght@0,100;0,300;0,400;0,500;0,700;0,900;1,100;1,300;1,400;1,500;1,700;1,900&display=swap'); body { color: var(--mud-palette-text-primary) !important; font-family: 'Be Vietnam Pro', sans-serif; font-weight:400; } h1, h2, h3, h4, h5, h6 {font-weight:900; } h1,h2,h3,h4,h5,h6 { font-family: 'Be Vietnam Pro', sans-serif;"},
            { "font_formats", "Bebas Neue=bebasneue;Roboto=roboto;"},
            { "toolbar", "undo redo | formatselect styles | bold italic | link | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent"},
            { "menubar", "edit format"},
            { "branding", "false"},
            { "height", $"{height.ToString()}" },
            { "block_formats", "Paragraph=p; Heading 4=h4; Heading 5=h5; Heading 6=h6"},
            { "format", "bold italic underline strikethrough superscript subscript codeformat | formats blockformats fontformats fontsizes align | forecolor backcolor | removeformat" }
        };
    public static readonly string ApiKey = "zdyvkmy3tesz0a0vsvxfnrn5039f1gt6ex9kp3b3xjiud6ah";
}