using Travaloud.Domain.Catalog.Pages;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.PageModals.Commands;

public class DeletePageModalRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }

    public DeletePageModalRequest(DefaultIdType id) => Id = id;
}

internal class DeletePageModalRequestHandler : IRequestHandler<DeletePageModalRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<PageModal> _repository;
    private readonly IStringLocalizer<DeletePageModalRequestHandler> _localizer;

    public DeletePageModalRequestHandler(IRepositoryFactory<PageModal> repository,
        IStringLocalizer<DeletePageModalRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<DefaultIdType> Handle(DeletePageModalRequest request, CancellationToken cancellationToken)
    {
        var pageModal = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = pageModal ?? throw new NotFoundException(_localizer["page.notfound"]);

        // Add Domain Events to be raised after the commit
        pageModal.DomainEvents.Add(EntityDeletedEvent.WithEntity(pageModal));

        await _repository.DeleteAsync(pageModal, cancellationToken);

        return request.Id;
    }
}