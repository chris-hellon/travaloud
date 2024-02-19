using Travaloud.Application.Catalog.JobVacancies.Commands;

namespace Travaloud.Application.Catalog.JobVacancies.Validation;

public class CreateJobVacancyRequestValidator : CustomValidator<CreateJobVacancyRequest>
{
    public CreateJobVacancyRequestValidator()
    {
        RuleFor(p => p.JobTitle)
            .NotEmpty();

        RuleFor(p => p.Location)
            .NotEmpty();
    }
}