using Travaloud.Domain.Catalog.SEO;

namespace Travaloud.Application.Catalog.Seo;

public class GetSeoRedirectsRequest  : PaginationFilter, IRequest<PaginationResponse<SeoRedirectDto>>
{
    
}

internal class GetSeoRedirectsRequestHandler  : IRequestHandler<GetSeoRedirectsRequest, PaginationResponse<SeoRedirectDto>>
{
    private readonly IRepositoryFactory<SeoRedirect> _repository;

    public GetSeoRedirectsRequestHandler(IRepositoryFactory<SeoRedirect> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<SeoRedirectDto>> Handle(GetSeoRedirectsRequest request, CancellationToken cancellationToken)
    {
        var spec = new SeoRedirectsSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}