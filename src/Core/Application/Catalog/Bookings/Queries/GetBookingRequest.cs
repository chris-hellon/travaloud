using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Queries;

public class GetBookingRequest : IRequest<BookingDetailsDto>
{
    public GetBookingRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class GetPropertyRequestHandler : IRequestHandler<GetBookingRequest, BookingDetailsDto>
{
    private readonly IRepositoryFactory<Booking> _repository;
    private readonly IStringLocalizer<GetPropertyRequestHandler> _localizer;

    public GetPropertyRequestHandler(IRepositoryFactory<Booking> repository,
        IStringLocalizer<GetPropertyRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<BookingDetailsDto> Handle(GetBookingRequest request, CancellationToken cancellationToken) =>
        await _repository.FirstOrDefaultAsync(
            new BookingByIdSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(_localizer["booking.notfound"], request.Id));
}