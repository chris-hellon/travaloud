using Travaloud.Application.Catalog.JobVacancies.Dto;

namespace Travaloud.Application.Catalog.JobVacanciesResponses.Dto;

public class JobVacancyResponseDto
{
    public DefaultIdType Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Name => $"{FirstName} {LastName}";
    public string Email { get; set; } = default!;
    public DateTime CreatedOn { get; set; } = default!;
    public string HowCanWeCollaborate { get; set; } = default!;
    public string EstimatedDates { get; set; } = default!;
    public string DestinationsVisited { get; set; } = default!;
    public string? Instagram { get; set; }
    public string? TikTok { get; set; }
    public string? YouTube { get; set; }
    public string? Portfolio { get; set; }
    public string? Equipment { get; set; }
    public JobVacancyDto JobVacancy { get; set; } = default!;
}