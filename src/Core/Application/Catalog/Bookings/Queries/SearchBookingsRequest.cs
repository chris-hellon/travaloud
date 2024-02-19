using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Queries;

public class SearchBookingsRequest : PaginationFilter, IRequest<PaginationResponse<BookingDto>>
{
    public string? Description { get; set; }
    public DateTime? BookingStartDate { get; set; }
    public DateTime? BookingEndDate { get; set; }
    public DateTime? TourStartDate { get; set; }
    public DateTime? TourEndDate { get; set; }
    public bool IsTours { get; set; }
    public DefaultIdType? TourId { get; set; }
    public DefaultIdType? PropertyId { get; set; }
}

public class SearchBookingsRequestHandler : IRequestHandler<SearchBookingsRequest, PaginationResponse<BookingDto>>
{
    private readonly IRepositoryFactory<Booking> _repository;

    public SearchBookingsRequestHandler(IRepositoryFactory<Booking> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<BookingDto>> Handle(SearchBookingsRequest request, CancellationToken cancellationToken)
    {
        var spec = new BookingsBySearchRequest(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}