using Travaloud.Domain.Catalog.SEO;

namespace Travaloud.Application.Catalog.Seo;

public class UpdateSeoRedirectRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public string Url { get; set; }
    public string RedirectUrl { get; set; }

    public UpdateSeoRedirectRequest()
    {
    }
}

internal class UpdateSeoRedirectRequestHandler : IRequestHandler<UpdateSeoRedirectRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<SeoRedirect> _repository;

    public UpdateSeoRedirectRequestHandler(IRepositoryFactory<SeoRedirect> repository)
    {
        _repository = repository;
    }

    public async Task<DefaultIdType> Handle(UpdateSeoRedirectRequest request, CancellationToken cancellationToken)
    {
        var seoRedirect = await _repository.GetByIdAsync(request.Id, cancellationToken) ?? throw new KeyNotFoundException();

        var updatedSeoRedirect = seoRedirect.Update(request.Url, request.RedirectUrl);
        
        await _repository.UpdateAsync(updatedSeoRedirect, cancellationToken);

        return seoRedirect.Id;
    }
}

public class UpdateSeoRedirectRequestValidator : CustomValidator<UpdateSeoRedirectRequest>
{
    public UpdateSeoRedirectRequestValidator(IRepositoryFactory<SeoRedirect> repository, IStringLocalizer<UpdateSeoRedirectRequestValidator> localizer)
    {
        RuleFor(p => p.Url)
            .NotEmpty();
        
        RuleFor(p => p.RedirectUrl)
            .NotEmpty();
    }
}