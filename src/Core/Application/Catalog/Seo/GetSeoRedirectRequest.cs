using Travaloud.Domain.Catalog.SEO;

namespace Travaloud.Application.Catalog.Seo;

public class GetSeoRedirectRequest : IRequest<SeoRedirectDto>
{
    public GetSeoRedirectRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

internal class GetSeoRedirectRequestHandler : IRequestHandler<GetSeoRedirectRequest, SeoRedirectDto>
{
    private readonly IRepositoryFactory<SeoRedirect> _repository;
    private readonly IStringLocalizer<GetSeoRedirectRequestHandler> _localizer;

    public GetSeoRedirectRequestHandler(IRepositoryFactory<SeoRedirect> repository, IStringLocalizer<GetSeoRedirectRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<SeoRedirectDto> Handle(GetSeoRedirectRequest request, CancellationToken cancellationToken)
    {
        var seoRedirect = await _repository.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException("Page Redirect not found");

        return seoRedirect.Adapt<SeoRedirectDto>();
    }
}