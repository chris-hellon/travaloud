namespace Travaloud.Domain.Catalog.JobVacancies;

public class JobVacancyResponse : AuditableEntity, IAggregateRoot
{
    public JobVacancyResponse(string firstName,
        string lastName,
        string email,
        string howCanWeCollaborate,
        string estimatedDates,
        string destinationsVisited,
        string? instagram,
        string? tikTok,
        string? youTube,
        string? portfolio,
        string? equipment,
        DefaultIdType jobVacancyId)
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
        JobVacancyId = jobVacancyId;
    }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string HowCanWeCollaborate { get; private set; }
    public string EstimatedDates { get; private set; }
    public string DestinationsVisited { get; private set; }
    public string? Instagram { get; private set; }
    public string? TikTok { get; private set; }
    public string? YouTube { get; private set; }
    public string? Portfolio { get; private set; }
    public string? Equipment { get; private set; }
    public DefaultIdType JobVacancyId { get; private set; }
    
    public virtual JobVacancy JobVacancy { get; set; }
}