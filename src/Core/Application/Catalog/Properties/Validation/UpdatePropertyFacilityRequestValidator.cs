using Travaloud.Application.Catalog.Properties.Commands;
using Travaloud.Application.Catalog.Properties.Specification;
using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Application.Catalog.Properties.Validation;

public class UpdatePropertyFacilityRequestValidator : CustomValidator<UpdatePropertyFacilityRequest>
{
    public UpdatePropertyFacilityRequestValidator(IRepositoryFactory<PropertyFacility> repo, IStringLocalizer<UpdatePropertyFacilityRequestValidator> localizer)
    {
        RuleFor(p => p.Title).NotEmpty();

        RuleFor(p => new { p.Title, p.PropertyId, p.Id })
            .MustAsync(async (room, ct) => await Validate(repo, room.Title, room.PropertyId, room.Id, ct))
            .WithMessage((_, name) => string.Format(localizer["propertyFacility.alreadyexists"], name));
    }

    private static async Task<bool> Validate(IRepositoryFactory<PropertyFacility> repo, string name, DefaultIdType propertyId, DefaultIdType? id, CancellationToken ct)
    {
        var propertyRoomResponse = await repo.SingleOrDefaultAsync(new PropertyFacilityByNameSpec(name, propertyId), ct);

        if (id.HasValue)
        {
            return propertyRoomResponse is not PropertyFacility existingTourDate || existingTourDate.Id == id;
        }
        else
        {
            return propertyRoomResponse is null;
        }
    }
}