using Travaloud.Domain.Catalog.Services;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Services.Commands;

public class DeleteServiceRequest : IRequest<DefaultIdType>
{
    public DeleteServiceRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class DeleteServiceRequestHandler : IRequestHandler<DeleteServiceRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Service> _repository;
    private readonly IStringLocalizer<DeleteServiceRequestHandler> _localizer;

    public DeleteServiceRequestHandler(IRepositoryFactory<Service> repository,
        IStringLocalizer<DeleteServiceRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<DefaultIdType> Handle(DeleteServiceRequest request, CancellationToken cancellationToken)
    {
        var booking = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = booking ?? throw new NotFoundException(_localizer["service.notfound"]);

        // Add Domain Events to be raised after the commit
        booking.DomainEvents.Add(EntityDeletedEvent.WithEntity(booking));

        await _repository.DeleteAsync(booking, cancellationToken);

        return request.Id;
    }
}