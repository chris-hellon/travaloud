namespace Travaloud.Domain.Catalog.JobVacancies;

public class JobVacancy : AuditableEntity, IAggregateRoot
{
    public JobVacancy(string location, string? jobTitle, string? description, string? callToAction)
    {
        Location = location;
        JobTitle = jobTitle;
        Description = description.FormatTextEditorString();
        CallToAction = callToAction;
    }

    public string Location { get; private set; }
    public string? JobTitle { get; private set; }
    public string? Description { get; private set; }
    public string? CallToAction { get; private set; }

    public virtual IList<JobVacancyResponse>? Responses { get; set; }

    public JobVacancy Update(string? location, string? jobTitle, string? description, string? callToAction)
    {
        if (location is not null && Location?.Equals(location) is not true) Location = location;
        if (jobTitle is not null && JobTitle?.Equals(jobTitle) is not true) JobTitle = jobTitle;
        if (description is not null && Description?.Equals(description) is not true) Description = description.FormatTextEditorString();;
        if (callToAction is not null && CallToAction?.Equals(callToAction) is not true) CallToAction = callToAction;
        return this;
    }
}