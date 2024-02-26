namespace Travaloud.Tenants.SharedRCL.Pages.Error;

public class IndexModel : TravaloudBasePageModel
{
    public override string MetaKeywords()
    {
        if (!StatusCodeValue.HasValue) return "error 500, server error, internal server error, website error";
        
        return StatusCodeValue.Value switch
        {
            404 => "error 404, page not found",
            401 => "error, unauthorized access, website error, 401 error",
            _ => "error 500, server error, internal server error, website error"
        };
    }

    public override string MetaDescription()
    {
        if (!StatusCodeValue.HasValue)
            return
                "Oops! Something went wrong with our website. Our team has been notified and we'll do our best to get this fixed as soon as possible. Please try again later.";
        
        return StatusCodeValue.Value switch
        {
            404 =>
                "Oops! Looks like the page you were trying to access does not exist. Please check the URL and try again. If you continue to experience issues, please contact us.",
            401 =>
                "Oops! It looks like you're trying to access a page that requires authorized access. Please log in or contact us if you think this is a mistake.",
            _ =>
                "Oops! Something went wrong with our website. Our team has been notified and we'll do our best to get this fixed as soon as possible. Please try again later."
        };
    }

    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    public string? ExceptionMessage { get; set; }
    public int? StatusCodeValue { get; set; }

    public async Task<IActionResult> OnGet(int? statusCode)
    {
        await OnGetDataAsync();

        StatusCodeValue = statusCode;

        var errorMessage = "An error occurred while processing your request.";

        if (statusCode.HasValue)
        {
            switch (statusCode.Value)
            {
                case 404:
                    ViewData["Title"] = "LOST IN THE MATRIX?";
                    errorMessage = "The page you are looking for doesn't exist or has been moved.";
                    break;
                case 401:
                    ViewData["Title"] = "UNAUTHORIZED";
                    errorMessage = "You are not authorized to access this page.";
                    break;
                default:
                    ViewData["Title"] = "ERROR";
                    errorMessage = "An error occurred while processing your request.";
                    break;
            }
        }

        ExceptionMessage = errorMessage;

        return Page();
    }
}