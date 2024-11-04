using Travaloud.Domain.Catalog.SEO;

namespace Travaloud.Application.Catalog.Seo;

public class CreateSeoRedirectRequest : IRequest<DefaultIdType>
{
    public string Url { get; set; }
    public string RedirectUrl { get; set; }

    public CreateSeoRedirectRequest()
    {
        
    }
}

internal class CreateSeoRedirectRequestHandler : IRequestHandler<CreateSeoRedirectRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<SeoRedirect> _repository;

    public CreateSeoRedirectRequestHandler(IRepositoryFactory<SeoRedirect> repository)
    {
        _repository = repository;
    }

    public async Task<DefaultIdType> Handle(CreateSeoRedirectRequest request, CancellationToken cancellationToken)
    {
        var seoRedirect = new SeoRedirect(request.Url, request.RedirectUrl);
        
        await _repository.AddAsync(seoRedirect, cancellationToken);

        return seoRedirect.Id;
    }
}

public class CreateSeoRedirectRequestValidator : CustomValidator<CreateSeoRedirectRequest>
{
    public CreateSeoRedirectRequestValidator(IRepositoryFactory<SeoRedirect> repository, IStringLocalizer<CreateSeoRedirectRequestValidator> localizer)
    {
        RuleFor(p => p.Url)
            .NotEmpty();
        
        RuleFor(p => p.RedirectUrl)
            .NotEmpty();
    }
}