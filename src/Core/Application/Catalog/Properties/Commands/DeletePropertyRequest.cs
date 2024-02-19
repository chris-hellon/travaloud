using Travaloud.Domain.Catalog.Properties;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Properties.Commands;

public class DeletePropertyRequest : IRequest<DefaultIdType>
{
    public DeletePropertyRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class DeletePropertyRequestHandler : IRequestHandler<DeletePropertyRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Property> _repository;
    private readonly IStringLocalizer<DeletePropertyRequestHandler> _localizer;

    public DeletePropertyRequestHandler(IRepositoryFactory<Property> repository,
        IStringLocalizer<DeletePropertyRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<DefaultIdType> Handle(DeletePropertyRequest request, CancellationToken cancellationToken)
    {
        var booking = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = booking ?? throw new NotFoundException(_localizer["property.notfound"]);

        // Add Domain Events to be raised after the commit
        booking.DomainEvents.Add(EntityDeletedEvent.WithEntity(booking));

        await _repository.DeleteAsync(booking, cancellationToken);

        return request.Id;
    }
}