namespace Travaloud.Tenants.SharedRCL.Models.EmailTemplates;

public class JoinOurCrewTemplateModel : EmailTemplateBaseModel
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string HowCanWeCollaborate { get; set; } = default!;
    public string EstimatedDates { get; set; } = default!;
    public string DestinationsVisited { get; set; } = default!;
    public string? Instagram { get; set; }
    public string? TikTok { get; set; }
    public string? YouTube { get; set; }
    public string? Portfolio { get; set; }
    public string? Equipment { get; set; }
    public string? JobVacancy { get; set; } = default!;
    public string? Location { get; set; } = default!;

    public JoinOurCrewTemplateModel()
    {
        
    }
    
    public JoinOurCrewTemplateModel(string tenantName, string? primaryBackgroundColor, string? secondaryBackgroundColor, string? headerBackgroundColor, string? textColor, string? logoImageUrl, string firstName, string lastName, string email, string howCanWeCollaborate, string estimatedDates, string destinationsVisited, string? instagram, string? tikTok, string? youTube, string? portfolio, string? equipment, string? jobVacancy, string? location) : base(tenantName, primaryBackgroundColor, secondaryBackgroundColor, headerBackgroundColor, textColor, logoImageUrl)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        HowCanWeCollaborate = howCanWeCollaborate;
        EstimatedDates = estimatedDates;
        DestinationsVisited = destinationsVisited;
        Instagram = instagram;
        TikTok = tikTok;
        YouTube = youTube;
        Portfolio = portfolio;
        Equipment = equipment;
        JobVacancy = jobVacancy;
        Location = location;
    }
}