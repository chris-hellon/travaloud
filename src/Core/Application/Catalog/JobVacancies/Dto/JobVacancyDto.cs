namespace Travaloud.Application.Catalog.JobVacancies.Dto;

public class JobVacancyDto
{
    public DefaultIdType Id { get; set; }
    public string Location { get; set; } = default!;
    public string JobTitle { get; set; } = default!;
    public string? Description { get; set; }
    public string? CallToAction { get; set; }
}