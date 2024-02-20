using Travaloud.Domain.Catalog.TravelGuides;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.TravelGuides.Commands;

public class DeleteTravelGuideRequest : IRequest<DefaultIdType>
{
    public DeleteTravelGuideRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class DeleteTravelGuideRequestHandler : IRequestHandler<DeleteTravelGuideRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<TravelGuide> _repository;
    private readonly IStringLocalizer<DeleteTravelGuideRequestHandler> _localizer;

    public DeleteTravelGuideRequestHandler(IRepositoryFactory<TravelGuide> repository,
        IStringLocalizer<DeleteTravelGuideRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<DefaultIdType> Handle(DeleteTravelGuideRequest request, CancellationToken cancellationToken)
    {
        var booking = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = booking ?? throw new NotFoundException(_localizer["travelGuide.notfound"]);

        // Add Domain Events to be raised after the commit
        booking.DomainEvents.Add(EntityDeletedEvent.WithEntity(booking));

        await _repository.DeleteAsync(booking, cancellationToken);

        return request.Id;
    }
}