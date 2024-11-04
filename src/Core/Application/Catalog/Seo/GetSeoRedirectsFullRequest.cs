using Travaloud.Domain.Catalog.SEO;

namespace Travaloud.Application.Catalog.Seo;


public class GetSeoRedirectsFullRequest  : IRequest<List<SeoRedirectDto>>
{
    
}

internal class GetSeoRedirectsFullRequestHandler  : IRequestHandler<GetSeoRedirectsFullRequest, List<SeoRedirectDto>>
{
    private readonly IRepositoryFactory<SeoRedirect> _repository;

    public GetSeoRedirectsFullRequestHandler(IRepositoryFactory<SeoRedirect> repository)
    {
        _repository = repository;
    }

    public async Task<List<SeoRedirectDto>> Handle(GetSeoRedirectsFullRequest request, CancellationToken cancellationToken)
    {
        var seoRedirects = await _repository.ListAsync(cancellationToken);

        return seoRedirects.Adapt<List<SeoRedirectDto>>();
    }
}