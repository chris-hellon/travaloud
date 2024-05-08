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

    string IEmailTemplateService.GenerateTourManifestEmail(string tourName,
        string primaryBackgroundColor,
        string logoImageUrl,
        DateTime startDate,
        int guestCount,
        int paidGuestCount,
        int unPaidGuestCount,
        string tenantName)
{
    return $@"
<!DOCTYPE html>
<html>
<head>
    <title></title>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8""/>
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge""/>
    <style type=""text/css"">
    body, table, td, a {{ -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; }}
    table, td {{ mso-table-lspace: 0pt; mso-table-rspace: 0pt; }}
    img {{ -ms-interpolation-mode: bicubic; }}
    
    img {{ border: 0; height: auto; line-height: 100%; outline: none; text-decoration: none; }}
    table {{ border-collapse: collapse !important; }}
    body {{ height: 100% !important; margin: 0 !important; padding: 0 !important; width: 100% !important; }}
    
    
    a[x-apple-data-detectors] {{
        color: inherit !important;
        text-decoration: none !important;
        font-size: inherit !important;
        font-family: inherit !important;
        font-weight: inherit !important;
        line-height: inherit !important;
    }}

div[style*=""margin: 16px 0;""] {{ margin: 0 !important; }}
</style>
</head>
<body style=""margin: 0 !important; padding: 0 !important; background-color: #eeeeee;"" bgcolor=""#eeeeee"">

<div style=""display: none; font-size: 1px; color: #fefefe; line-height: 1px; font-family: Open Sans, Helvetica, Arial, sans-serif; max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden;"">
    {tourName} manifest
</div>

<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
    <tr>
        <td align=""center"" style=""background-color: #eeeeee;"" bgcolor=""#eeeeee"">

            <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width:600px;"">
                <tr>
                    <td align=""center"" valign=""top"" style=""font-size:0; padding: 35px;"" bgcolor=""{primaryBackgroundColor}"">

                        <div style=""display:inline-block; max-width:50%; min-width:100px; vertical-align:top; width:100%;"">
                            <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width:300px;"">
                                <tr>
                                    <td align=""center"" valign=""top"" style=""font-family: Open Sans, Helvetica, Arial, sans-serif; font-size: 48px; font-weight: 400; line-height: 48px;"">
                                        <table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"">
                                            <tr>
                                                <td style=""font-family: Open Sans, Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 24px;"">
                                                    <a href=""#"" target=""_blank"" style=""color: #ffffff; text-decoration: none;"">
                                                        <img src=""{logoImageUrl}"" width=""100"" height=""100"" style=""display: block; border: 0px;""/>
                                                    </a>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>

                    </td>
                </tr>
                <tr>
                    <td align=""center"" style=""padding: 35px 35px 20px 35px; background-color: #ffffff;"" bgcolor=""#ffffff"">
                        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width:600px;"">
                            <tr>
                                <td align=""center"" style=""font-family: Open Sans, Helvetica, Arial, sans-serif; font-size: 16px; font-weight: 400; line-height: 24px; padding-top: 25px;"">
                                    <h2 style=""font-size: 30px; font-weight: 800; line-height: 36px; color: #333333; margin: 0;"">
                                        {tourName} manifest
                                    </h2>
                                </td>
                            </tr>
                            <tr>
                                <td align=""left"" style=""font-family: Open Sans, Helvetica, Arial, sans-serif; font-size: 16px; font-weight: 400; line-height: 24px; padding-top: 10px;"">
                                    <p style=""font-size: 16px; font-weight: 400; line-height: 24px; color: #777777;"">
                                        Please find attached Today's Tour Manifest for {tourName} - {startDate.TimeOfDay}.
                                    </p>
                                </td>
                            </tr>
                            <tr>
                                <td align=""left"" style=""font-family: Open Sans, Helvetica, Arial, sans-serif; font-size: 16px; font-weight: 400; line-height: 24px; padding-top: 10px;"">
                                    <p style=""font-size: 16px; font-weight: 400; line-height: 24px; color: #777777;"">
                                        There are a total of {guestCount} guests in attendance. {paidGuestCount} guests have paid & {unPaidGuestCount} guests have yet to pay.
                                    </p>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>

                <tr>
                    <td align=""center"" style=""padding: 35px; background-color: #ffffff;"" bgcolor=""#ffffff"">
                        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width:600px;"">
                            <tr>
                                <td align=""center"">
                                    <img src=""{logoImageUrl}"" width=""37"" height=""37"" style=""display: block; border: 0px;"" alt=""footer logo""/>
                                </td>
                            </tr>
                            <tr>
                                <td align=""center"" style=""font-family: Open Sans, Helvetica, Arial, sans-serif; font-size: 14px; font-weight: 400; line-height: 24px; padding: 5px 0 10px 0;"">
                                    <p style=""font-size: 14px; font-weight: 800; line-height: 18px; color: #333333;"">
                                        {tenantName}
                                    </p>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
</body>
</html>";
}

}