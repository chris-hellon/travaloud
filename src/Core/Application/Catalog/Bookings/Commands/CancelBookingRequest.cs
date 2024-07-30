using System.Data;
using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Bookings.Commands;

public class CancelBookingRequest : IRequest<bool>
{
    public DefaultIdType Id { get; set; }

    public CancelBookingRequest(DefaultIdType id)
    {
        Id = id;
    }
}

internal class CancelBookingRequestHandler : IRequestHandler<CancelBookingRequest, bool>
{
    private readonly IDapperRepository _dapperRepository;
    private readonly IRepositoryFactory<Booking> _repository;
    private readonly IRepositoryFactory<TourDate> _tourDateRepository;
    private readonly IStringLocalizer<DeleteBookingRequestHandler> _localizer;
    
    public CancelBookingRequestHandler(IDapperRepository dapperRepository, IRepositoryFactory<Booking> repository, IRepositoryFactory<TourDate> tourDateRepository, IStringLocalizer<DeleteBookingRequestHandler> localizer)
    {
        _dapperRepository = dapperRepository;
        _repository = repository;
        _tourDateRepository = tourDateRepository;
        _localizer = localizer;
    }

    public async Task<bool> Handle(CancelBookingRequest request, CancellationToken cancellationToken)
    {
        // var booking = await _repository.SingleOrDefaultAsync(new BookingByIdSpec(request.Id), cancellationToken);
        //
        // _ = booking ?? throw new NotFoundException(_localizer["booking.notfound"]);
        //
        // foreach (var item in booking.Items)
        // {
        //     if (!item.TourDateId.HasValue) continue;
        //     
        //     var date = await _tourDateRepository.GetByIdAsync(item.TourDateId.Value, cancellationToken);
        //
        //     if (date == null) continue;
        //     
        //     date.AvailableSpaces += (item.Guests?.Count + 1) ?? 1;
        //     await _tourDateRepository.UpdateAsync(date, cancellationToken);
        // }
        
        await _dapperRepository.ExecuteAsync("CancelBooking", new
        {
            BookingId = request.Id
        }, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);

        return true;
    }
}
