namespace Travaloud.Application.Catalog.Seo;

public class UpdateSeoRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public string? Breadcrumbs { get; set; }
    public string? SearchAction { get; set; }
}

internal class UpdateSeoRequestHandler : IRequestHandler<UpdateSeoRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Domain.Catalog.SEO.Seo> _repository;

    public UpdateSeoRequestHandler(IRepositoryFactory<Domain.Catalog.SEO.Seo> repository)
    {
        _repository = repository;
    }

    public async Task<DefaultIdType> Handle(UpdateSeoRequest request, CancellationToken cancellationToken)
    {
        var seo = await _repository.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException(nameof(Domain.Catalog.SEO.Seo));

        var updatedSeo = seo.Update(request.Breadcrumbs, request.SearchAction);
        
        await _repository.UpdateAsync(seo, cancellationToken);

        return updatedSeo.Id;
    }
}