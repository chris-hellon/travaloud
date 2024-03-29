namespace Travaloud.Application.Identity.Roles;

public class CreateOrUpdateRoleRequest
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class CreateOrUpdateRoleRequestValidator : CustomValidator<CreateOrUpdateRoleRequest>
{
    public CreateOrUpdateRoleRequestValidator(IRoleService roleService, IStringLocalizer<CreateOrUpdateRoleRequestValidator> localizer) =>
        RuleFor(r => r.Name)
            .NotEmpty()
            .MustAsync(async (role, name, _) => name != null && !await roleService.ExistsAsync(name, role.Id))
                .WithMessage(localizer["Similar Role already exists."]);
}