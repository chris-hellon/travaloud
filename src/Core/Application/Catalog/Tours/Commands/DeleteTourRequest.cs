using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Domain.Catalog.Tours;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Tours.Commands;

public class DeleteTourRequest : IRequest<DefaultIdType>
{
    public DeleteTourRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class DeleteTourRequestHandler : IRequestHandler<DeleteTourRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Tour> _repository;
    private readonly IStringLocalizer<DeleteTourRequestHandler> _localizer;
    private readonly IRepositoryFactory<BookingItem> _bookingItemsRepository;

    public DeleteTourRequestHandler(IRepositoryFactory<Tour> repository,
        IStringLocalizer<DeleteTourRequestHandler> localizer, 
        IRepositoryFactory<BookingItem> bookingItemsRepository)
    {
        _repository = repository;
        _localizer = localizer;
        _bookingItemsRepository = bookingItemsRepository;
    }

    public async Task<DefaultIdType> Handle(DeleteTourRequest request, CancellationToken cancellationToken)
    {
        var tour = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = tour ?? throw new NotFoundException(_localizer["tour.notfound"]);

        var outstandingBookings =
            await _bookingItemsRepository.AnyAsync(new BookingItemsByTourIdSpec(request.Id), cancellationToken);

        if (outstandingBookings)
            throw new CustomException("Unable to delete this Tour as there are future bookings associated with it.");
        
        // Add Domain Events to be raised after the commit
        tour.DomainEvents.Add(EntityDeletedEvent.WithEntity(tour));

        await _repository.DeleteAsync(tour, cancellationToken);

        return request.Id;
    }
}