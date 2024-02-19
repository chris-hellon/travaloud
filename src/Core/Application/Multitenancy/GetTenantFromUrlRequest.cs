namespace Travaloud.Application.Multitenancy;

public class GetTenantFromUrlRequest : IRequest<TenantDto>
{
    public string Url { get; set; } = default!;

    public GetTenantFromUrlRequest(string url) => Url = url;
}

public class GetTenantFromUrlRequestValidator : CustomValidator<GetTenantFromUrlRequest>
{
    public GetTenantFromUrlRequestValidator() =>
        RuleFor(t => t.Url)
            .NotEmpty();
}

public class GetTenantFromUrlRequestHandler : IRequestHandler<GetTenantFromUrlRequest, TenantDto>
{
    private readonly ITenantService _tenantService;

    public GetTenantFromUrlRequestHandler(ITenantService tenantService) => _tenantService = tenantService;

    public Task<TenantDto> Handle(GetTenantFromUrlRequest request, CancellationToken cancellationToken) =>
        _tenantService.GetByUrlAsync(request.Url);
}