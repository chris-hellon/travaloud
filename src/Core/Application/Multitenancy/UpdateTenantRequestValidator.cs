namespace Travaloud.Application.Multitenancy;

public class UpdateTenantRequestValidator : CustomValidator<UpdateTenantRequest>
{
    public UpdateTenantRequestValidator(
        ITenantService tenantService,
        IStringLocalizer<UpdateTenantRequestValidator> localizer,
        IConnectionStringValidator connectionStringValidator)
    {
        RuleFor(t => t.Id).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (id, _) => await tenantService.ExistsWithIdAsync(id))
            .WithMessage((_, id) => string.Format(localizer["tenant.notfound"], id));

        RuleFor(t => t.ConnectionString).Cascade(CascadeMode.Stop)
            .Must((_, cs) => string.IsNullOrWhiteSpace(cs) || connectionStringValidator.TryValidate(cs))
            .WithMessage(localizer["invalid.connectionstring"]);

        RuleFor(t => t.AdminEmail).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress();
    }
}