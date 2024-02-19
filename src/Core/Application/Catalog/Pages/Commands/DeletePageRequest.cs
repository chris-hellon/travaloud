using Travaloud.Domain.Catalog.Pages;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Pages.Commands;

public class DeletePageRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }

    public DeletePageRequest(DefaultIdType id)
    {
        Id = id;
    }
}

internal class DeletePageRequestHandler : IRequestHandler<DeletePageRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Page> _repository;
    private readonly IStringLocalizer<DeletePageRequestHandler> _localizer;

    public DeletePageRequestHandler(IRepositoryFactory<Page> repository,
        IStringLocalizer<DeletePageRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<DefaultIdType> Handle(DeletePageRequest request, CancellationToken cancellationToken)
    {
        var page = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = page ?? throw new NotFoundException(_localizer["page.notfound"]);

        // Add Domain Events to be raised after the commit
        page.DomainEvents.Add(EntityDeletedEvent.WithEntity(page));

        await _repository.DeleteAsync(page, cancellationToken);

        return request.Id;
    }
}