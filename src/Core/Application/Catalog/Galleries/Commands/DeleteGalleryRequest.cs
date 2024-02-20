using Travaloud.Domain.Catalog.Galleries;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Galleries.Commands;

public class DeleteGalleryRequest : IRequest<DefaultIdType>
{
    public DeleteGalleryRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class DeleteTravelGuideRequestHandler : IRequestHandler<DeleteGalleryRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Gallery> _repository;
    private readonly IStringLocalizer<DeleteTravelGuideRequestHandler> _localizer;

    public DeleteTravelGuideRequestHandler(IRepositoryFactory<Gallery> repository,
        IStringLocalizer<DeleteTravelGuideRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<DefaultIdType> Handle(DeleteGalleryRequest request, CancellationToken cancellationToken)
    {
        var gallery = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = gallery ?? throw new NotFoundException(_localizer["travelGuide.notfound"]);

        // Add Domain Events to be raised after the commit
        gallery.DomainEvents.Add(EntityDeletedEvent.WithEntity(gallery));

        await _repository.DeleteAsync(gallery, cancellationToken);

        return request.Id;
    }
}