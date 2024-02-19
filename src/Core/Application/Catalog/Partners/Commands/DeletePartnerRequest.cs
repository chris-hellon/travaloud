using Travaloud.Domain.Catalog.Partners;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Partners.Commands;

public class DeletePartnerRequest : IRequest<DefaultIdType>
{
    public DeletePartnerRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class DeletePartnerRequestHandler : IRequestHandler<DeletePartnerRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Partner> _repository;
    private readonly IStringLocalizer<DeletePartnerRequestHandler> _localizer;

    public DeletePartnerRequestHandler(IRepositoryFactory<Partner> repository,
        IStringLocalizer<DeletePartnerRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<DefaultIdType> Handle(DeletePartnerRequest request, CancellationToken cancellationToken)
    {
        var booking = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = booking ?? throw new NotFoundException(_localizer["product.notfound"]);

        // Add Domain Events to be raised after the commit
        booking.DomainEvents.Add(EntityDeletedEvent.WithEntity(booking));

        await _repository.DeleteAsync(booking, cancellationToken);

        return request.Id;
    }
}