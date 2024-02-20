using Travaloud.Application.Catalog.Properties.Commands;
using Travaloud.Application.Catalog.Properties.Specification;
using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Application.Catalog.Properties.Validation;

public class UpdatePropertyRoomRequestValidator : CustomValidator<PropertyRoomRequest>
{
    public UpdatePropertyRoomRequestValidator(IRepositoryFactory<PropertyRoom> repo, IStringLocalizer<UpdatePropertyRoomRequestValidator> localizer)
    {
        RuleFor(p => p.Name).NotEmpty();
        RuleFor(p => p.Description).NotEmpty();

        RuleFor(p => new { p.Name, p.PropertyId, p.Id })
            .MustAsync(async (room, ct) => await Validate(repo, room.Name, room.PropertyId, room.Id, ct))
            .WithMessage((_, name) => string.Format(localizer["propertyRoom.alreadyexists"], name));

        RuleFor(p => p.Image)
            .SetNonNullableValidator(new FileUploadRequestValidator());
    }

    private static async Task<bool> Validate(IRepositoryFactory<PropertyRoom> repo, string name, DefaultIdType propertyId, DefaultIdType? id, CancellationToken ct)
    {
        var propertyRoomResponse = await repo.SingleOrDefaultAsync(new PropertyRoomByNameSpec(name, propertyId), ct);

        if (id.HasValue)
        {
            return propertyRoomResponse is null || propertyRoomResponse.Id == id;
        }

        return propertyRoomResponse is null;
    }
}