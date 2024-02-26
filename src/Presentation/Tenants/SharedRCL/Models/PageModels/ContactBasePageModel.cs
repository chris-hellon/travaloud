using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Http.Extensions;
using Travaloud.Application.Common.Mailing;

namespace Travaloud.Tenants.SharedRCL.Models.PageModels;

[ValidateReCaptcha]
public abstract class ContactBasePageModel<T, T2> : TravaloudBasePageModel
{
    public virtual string Subject()
    {
        return "Website Enquiry";
    }

    public virtual string TemplateName()
    {
        return "ContactTemplate";
    }
    
    public Func<Task>? SubmitFunction { get; set; }
        
    public virtual async Task<IActionResult> OnGetAsync(string? tourName = null)
    {
        await base.OnGetDataAsync();

        ModelState.Clear();
        return Page();
    }


    public virtual async Task<IActionResult> OnPostAsync([FromServices] IReCaptchaService service, T model, T2 formModel)
    {
        if (!ModelState.IsValid)
        {
            ModelState.Remove("Name");
            ModelState.Remove("Email");
            ModelState.Remove("Message");
            ModelState.Remove("Gender");
            ModelState.Remove("Surname");
            ModelState.Remove("Password");
            ModelState.Remove("FirstName");
            ModelState.Remove("Nationality");
            ModelState.Remove("DateOfBirth");
            ModelState.Remove("PhoneNumber");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("Date");
            ModelState.Remove("NumberOfPeople");
            ModelState.Remove("TourName");
            ModelState.Remove("TourDate");
            ModelState.Remove("PropertyId");
            ModelState.Remove("CheckInDate");
            ModelState.Remove("Description");
            ModelState.Remove("FriendlyUrl");
            ModelState.Remove("CheckOutDate");
            ModelState.Remove("GuestQuantity");
            ModelState.Remove("ContactNumber");
            ModelState.Remove("Location");
            ModelState.Remove("JobTitle");
            ModelState.Remove("LastName");
            ModelState.Remove("JobVacancy");
            ModelState.Remove("EstimatedDates");
            ModelState.Remove("DestinationsVisited");
            ModelState.Remove("HowCanWeCollaborate");
        }

        if (!ModelState.IsValid)
        {
            StatusMessage = "<p>Please complete the Google Captcha to send your message.</p>";
            StatusSeverity = "danger";
            return Redirect(Request.GetEncodedUrl());
        }

        if (formModel?.GetType().GetProperty("HoneyPot") != null)
        {
            var honeyPotProperty = formModel.GetType().GetProperty("HoneyPot");
            var honeyPotValue = honeyPotProperty?.GetValue(formModel, null);
            var honeyPot = honeyPotValue?.ToString();

            if (!string.IsNullOrEmpty(honeyPot))
            {
                StatusMessage = "<p>Suspicious activity detected.</p>";
                StatusSeverity = "danger";
                return Redirect(Request.GetEncodedUrl());
            }
        }

        var emailHtml = await RazorPartialToStringRenderer?.RenderPartialToStringAsync($"/Pages/EmailTemplates/{TemplateName()}.cshtml", model);

        if (MailSettings?.ToAddress != null)
        {
            var mailRequest = new MailRequest(
                to: [MailSettings.ToAddress],
                subject: Subject(),
                body: emailHtml,
                bcc: [MailSettings.BccAddress.ToString()]);
        
            await MailService.SendAsync(mailRequest);
        }

        StatusMessage = "<p>Your request has been sent successfully.</p><p>A member of our team will contact you shortly.</p>";

        if (SubmitFunction != null)
            await SubmitFunction.Invoke();

        ModelState.Clear();

        return Redirect(Request.GetEncodedUrl());
    }
}