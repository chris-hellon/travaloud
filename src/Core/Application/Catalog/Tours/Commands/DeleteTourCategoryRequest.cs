using Travaloud.Domain.Catalog.Tours;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Tours.Commands;

public class DeleteTourCategoryRequest : IRequest<DefaultIdType>
{
    public DeleteTourCategoryRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class DeleteTourCategoryRequestHandler : IRequestHandler<DeleteTourCategoryRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<TourCategory> _repository;
    private readonly IStringLocalizer<DeleteTourCategoryRequestHandler> _localizer;

    public DeleteTourCategoryRequestHandler(IRepositoryFactory<TourCategory> repository,
        IStringLocalizer<DeleteTourCategoryRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<DefaultIdType> Handle(DeleteTourCategoryRequest request, CancellationToken cancellationToken)
    {
        var tour = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = tour ?? throw new NotFoundException(_localizer["tourCategory.notfound"]);

        // Add Domain Events to be raised after the commit
        tour.DomainEvents.Add(EntityDeletedEvent.WithEntity(tour));

        await _repository.DeleteAsync(tour, cancellationToken);

        return request.Id;
    }
}