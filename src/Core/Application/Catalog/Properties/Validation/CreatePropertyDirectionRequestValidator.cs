using Travaloud.Application.Catalog.Properties.Commands;

namespace Travaloud.Application.Catalog.Properties.Validation;

public class CreatePropertyDirectionRequestValidator : CustomValidator<CreatePropertyDirectionRequest>
{
    public CreatePropertyDirectionRequestValidator()
    {
        RuleFor(p => p.Title).NotEmpty();

        RuleFor(b => b.Content)
            .NotEmpty()
            .Must(ValidateItems)
            .WithMessage("One or more items have validation errors.");
    }

    private bool ValidateItems(IEnumerable<CreatePropertyDirectionContentRequest> items)
    {
        var validator = new CreatePropertyDirectionContentRequestValidator();
        var validationResults = items.Select(item => validator.Validate(item));
        var aggregateErrors = validationResults.SelectMany(result => result.Errors).ToList();
        if (aggregateErrors.Any())
        {
            foreach (var error in aggregateErrors)
            {
                error.PropertyName = $"Content[{error.PropertyName}]";
            }

            throw new ValidationException(aggregateErrors);
        }

        return true;
    }
}