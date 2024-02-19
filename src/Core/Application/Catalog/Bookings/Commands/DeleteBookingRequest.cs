using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Domain.Catalog.Tours;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Bookings.Commands;

public class DeleteBookingRequest : IRequest<DefaultIdType>
{
    public DeleteBookingRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class DeleteBookingRequestHandler : IRequestHandler<DeleteBookingRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<BookingItem> _bookingItemsRepository;
    private readonly IRepositoryFactory<Booking> _repository;
    private readonly IRepositoryFactory<TourDate> _tourDateRepository;
    private readonly IStringLocalizer<DeleteBookingRequestHandler> _localizer;

    public DeleteBookingRequestHandler(IRepositoryFactory<Booking> repository,
        IRepositoryFactory<TourDate> tourDateRepository,
        IRepositoryFactory<BookingItem> bookingItemsRepository,
        IStringLocalizer<DeleteBookingRequestHandler> localizer)
    {
        _repository = repository;
        _tourDateRepository = tourDateRepository;
        _localizer = localizer;
        _bookingItemsRepository = bookingItemsRepository;
    }

    public async Task<DefaultIdType> Handle(DeleteBookingRequest request, CancellationToken cancellationToken)
    {
        var booking = await _repository.SingleOrDefaultAsync(new BookingByIdSpec(request.Id), cancellationToken);

        _ = booking ?? throw new NotFoundException(_localizer["booking.notfound"]);

        foreach (var item in booking.Items)
        {
            if (!item.TourDateId.HasValue) continue;
            
            var date = await _tourDateRepository.GetByIdAsync(item.TourDateId.Value, cancellationToken);

            if (date == null) continue;
            
            date.AvailableSpaces++;
            await _tourDateRepository.UpdateAsync(date, cancellationToken);
        }

        // Add Domain Events to be raised after the commit
        booking.DomainEvents.Add(EntityDeletedEvent.WithEntity(booking));

        await _repository.DeleteAsync(booking, cancellationToken);

        return request.Id;
    }
}