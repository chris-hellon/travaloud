using Travaloud.Domain.Catalog.SEO;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Seo;

public class DeleteSeoRedirectRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }

    public DeleteSeoRedirectRequest(DefaultIdType id)
    {
        Id = id;
    }
}

internal class DeleteSeoRedirectRequestHandler : IRequestHandler<DeleteSeoRedirectRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<SeoRedirect> _repository;
    private readonly IStringLocalizer<DeleteSeoRedirectRequestHandler> _localizer;

    public DeleteSeoRedirectRequestHandler(IRepositoryFactory<SeoRedirect> repository,
        IStringLocalizer<DeleteSeoRedirectRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<DefaultIdType> Handle(DeleteSeoRedirectRequest request, CancellationToken cancellationToken)
    {
        var page = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = page ?? throw new NotFoundException(_localizer["page.notfound"]);

        // Add Domain Events to be raised after the commit
        page.DomainEvents.Add(EntityDeletedEvent.WithEntity(page));

        await _repository.DeleteAsync(page, cancellationToken);

        return request.Id;
    }
}