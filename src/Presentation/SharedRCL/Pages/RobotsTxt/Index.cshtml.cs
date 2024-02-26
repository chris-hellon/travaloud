using System.Text;

namespace Travaloud.Tenants.SharedRCL.Pages.RobotsTxt;

public class IndexModel : PageModel
{
    public ContentResult OnGet()
    {
        var url = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";

            

        var sb = new StringBuilder();
        sb.AppendLine("User-agent: *")
            .AppendLine("Disallow:");

        if (url.Contains("staging")) sb.Append(" /");

        sb.Append("Sitemap: ")
            .Append(this.Request.Scheme)
            .Append("://")
            .Append(this.Request.Host)
            .AppendLine("/sitemap.xml");

        return this.Content(sb.ToString(), "text/plain", Encoding.UTF8);
    }
}