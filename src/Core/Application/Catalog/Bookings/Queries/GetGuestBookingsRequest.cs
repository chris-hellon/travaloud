using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Queries;

public class GetGuestBookingsRequest : PaginationFilter, IRequest<List<BookingDto>>
{
    public string GuestId { get; set; } = default!;

    public GetGuestBookingsRequest(string guestId) => GuestId = guestId;
}

public class GetGuestBookingsRequestHandler : IRequestHandler<GetGuestBookingsRequest, List<BookingDto>>
{
    private readonly IRepositoryFactory<Booking> _repository;

    public GetGuestBookingsRequestHandler(IRepositoryFactory<Booking> repository)
    {
        _repository = repository;
    }

    public async Task<List<BookingDto>> Handle(GetGuestBookingsRequest request, CancellationToken cancellationToken)
    {
        var spec = new BookingsByGuestRequest(request);
        var guestList = await _repository.PaginatedListAsync(spec, 1, 99999, cancellationToken: cancellationToken);

        return guestList.Data;
    }
}