namespace Travaloud.Application.Catalog.Seo;

public class GetSeoRequest : IRequest<SeoDetailsDto?>
{
    
}

internal class GetSeoRequestHandler : IRequestHandler<GetSeoRequest, SeoDetailsDto?>
{
    private readonly IRepositoryFactory<Domain.Catalog.SEO.Seo> _repository;
    private readonly IStringLocalizer<GetSeoRequestHandler> _localizer;

    public GetSeoRequestHandler(IRepositoryFactory<Domain.Catalog.SEO.Seo> repository,
        IStringLocalizer<GetSeoRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<SeoDetailsDto?> Handle(GetSeoRequest request, CancellationToken cancellationToken) {
        var seoDetails = await _repository.FirstOrDefaultAsync(new GetSeoSpec(), cancellationToken);

        if (seoDetails != null) return seoDetails;
        
        var newSeo = new Domain.Catalog.SEO.Seo();
        await _repository.AddAsync(newSeo, cancellationToken);
            
        seoDetails = await _repository.FirstOrDefaultAsync(new GetSeoSpec(), cancellationToken);

        return seoDetails;
    }
}