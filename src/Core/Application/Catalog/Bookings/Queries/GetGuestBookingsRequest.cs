using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Queries;

public class GetGuestBookingsRequest : PaginationFilter, IRequest<IEnumerable<BookingDto>>
{
    public string GuestId { get; set; } = default!;

    public GetGuestBookingsRequest(string guestId) => GuestId = guestId;
}

public class GetGuestBookingsRequestHandler : IRequestHandler<GetGuestBookingsRequest, IEnumerable<BookingDto>>
{
    private readonly IRepositoryFactory<Booking> _repository;

    public GetGuestBookingsRequestHandler(IRepositoryFactory<Booking> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<BookingDto>> Handle(GetGuestBookingsRequest request, CancellationToken cancellationToken)
    {
        var spec = new BookingsByGuestSpec(request);
        var guestList = await _repository.PaginatedListAsync(spec, 1, 99999, cancellationToken: cancellationToken);

        return guestList.Data;
    }
}