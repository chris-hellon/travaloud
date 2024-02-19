using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Domain.Common.Events;
using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Bookings.Commands;

public class DeleteBookingItemRequest : IRequest<DefaultIdType>
{
    public DeleteBookingItemRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class DeleteBookingItemRequestHandler : IRequestHandler<DeleteBookingItemRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<BookingItem> _repository;
    private readonly IRepositoryFactory<TourDate> _tourDateRepository;
    private readonly IStringLocalizer<DeleteBookingItemRequestHandler> _localizer;

    public DeleteBookingItemRequestHandler(IRepositoryFactory<BookingItem> repository,
        IRepositoryFactory<TourDate> tourDateRepository,
        IStringLocalizer<DeleteBookingItemRequestHandler> localizer)
    {
        _repository = repository;
        _tourDateRepository = tourDateRepository;
        _localizer = localizer;
    }

    public async Task<DefaultIdType> Handle(DeleteBookingItemRequest request, CancellationToken cancellationToken)
    {
        var bookingItem = await _repository.SingleOrDefaultAsync(new BookingItemByIdSpec(request.Id), cancellationToken);

        _ = bookingItem ?? throw new NotFoundException(_localizer["bookingItem.notfound"]);

        if (bookingItem.TourDate != null)
        {
            bookingItem.TourDate.AvailableSpaces++;
            await _tourDateRepository.UpdateAsync(bookingItem.TourDate, cancellationToken);
        }

        // Add Domain Events to be raised after the commit
        bookingItem.DomainEvents.Add(EntityDeletedEvent.WithEntity(bookingItem));

        await _repository.DeleteAsync(bookingItem, cancellationToken);

        return request.Id;
    }
}