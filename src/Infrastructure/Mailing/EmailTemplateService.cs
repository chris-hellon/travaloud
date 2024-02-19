using System.Text;
using RazorEngineCore;
using Travaloud.Application.Common.Mailing;

namespace Travaloud.Infrastructure.Mailing;

public class EmailTemplateService : IEmailTemplateService
{
    public string GenerateEmailTemplate<T>(string templateName, T mailTemplateModel)
    {
        var template = GetTemplate(templateName);

        var razorEngine = new RazorEngine();
        var modifiedTemplate = razorEngine.Compile(template);

        return modifiedTemplate.Run(mailTemplateModel);
    }

    private static string GetTemplate(string templateName)
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        
        const string emailTemplatesDirectory = "EmailTemplates";
        var filePath = Path.Combine(emailTemplatesDirectory, $"{templateName}.cshtml");

        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs, Encoding.Default);
        var mailText = sr.ReadToEnd();
        sr.Close();

        return mailText;
    }
}