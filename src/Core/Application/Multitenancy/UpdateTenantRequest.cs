namespace Travaloud.Application.Multitenancy;

public class UpdateTenantRequest : IRequest<string>
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? ConnectionString { get; set; }
    public string AdminEmail { get; set; } = default!;
    public string? Issuer { get; set; }
    public string? Url { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? LogoImageUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public string? PrimaryHoverColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string? SecondaryHoverColor { get; set; }
    public string? TeritaryColor { get; set; }
    public string? TeritaryHoverColor { get; set; }
    public string? HeaderFontWoffUrl { get; set; }
    public string? HeaderFontWoff2Url { get; set; }
    public string? HeaderFont { get; set; }
    public string? BodyFontWoffUrl { get; set; }
    public string? BodyFontWoff2Url { get; set; }
    public string? BodyFont { get; set; }
}

public class UpdateTenantRequestHandler : IRequestHandler<UpdateTenantRequest, string>
{
    private readonly ITenantService _tenantService;

    public UpdateTenantRequestHandler(ITenantService tenantService) => _tenantService = tenantService;

    public Task<string> Handle(UpdateTenantRequest request, CancellationToken cancellationToken) =>
        _tenantService.UpdateAsync(request, cancellationToken);
}