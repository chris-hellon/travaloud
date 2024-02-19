using Travaloud.Application.Catalog.JobVacancies.Commands;

namespace Travaloud.Application.Catalog.JobVacancies.Validation;

public class UpdateJobVacancyRequestValidator : CustomValidator<UpdateJobVacancyRequest>
{
    public UpdateJobVacancyRequestValidator()
    {
        RuleFor(p => p.JobTitle)
            .NotEmpty();

        RuleFor(p => p.Location)
            .NotEmpty();
    }
}