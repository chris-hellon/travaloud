using Travaloud.Application.Catalog.Properties.Commands;
using Travaloud.Application.Catalog.Properties.Specification;
using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Application.Catalog.Properties.Validation;

public class UpdatePropertyDirectionRequestValidator : CustomValidator<PropertyDirectionRequest>
{
    public UpdatePropertyDirectionRequestValidator(IRepositoryFactory<PropertyDirection> repo, IStringLocalizer<UpdatePropertyDirectionRequestValidator> localizer)
    {
        RuleFor(p => p.Title).NotEmpty();

        RuleFor(p => new { p.Title, p.PropertyId, p.Id })
            .MustAsync(async (room, ct) => await Validate(repo, room.Title, room.PropertyId, room.Id, ct))
            .WithMessage((_, name) => string.Format(localizer["propertyDirection.alreadyexists"], name));

        RuleFor(b => b.Content)
            .NotEmpty()
            .Must(ValidateItems)
            .WithMessage("One or more items have validation errors.");
    }

    private bool ValidateItems(IEnumerable<PropertyDirectionContentRequest> items)
    {
        var validator = new UpdateBookingDirectionContentValidator();
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

    private static async Task<bool> Validate(IRepositoryFactory<PropertyDirection> repo, string name, DefaultIdType propertyId, DefaultIdType? id, CancellationToken ct)
    {
        var propertyRoomResponse = await repo.SingleOrDefaultAsync(new PropertyDirectionByNameSpec(name, propertyId), ct);

        if (id.HasValue)
        {
            return propertyRoomResponse is not PropertyDirection existingTourDate || existingTourDate.Id == id;
        }
        else
        {
            return propertyRoomResponse is null;
        }
    }
}