using System.Data;
using Travaloud.Application.Catalog.Bookings.Dto;

namespace Travaloud.Application.Catalog.Bookings.Queries;

public class StaffBookingsByDateRangeRequest : PaginationFilter, IRequest<PaginationResponse<StaffBookingDto>>
{
    public string TenantId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}

internal class StaffBookingsByDateRangeRequestHandler : IRequestHandler<StaffBookingsByDateRangeRequest, PaginationResponse<StaffBookingDto>>
{
    private readonly IDapperRepository _repository;

    public StaffBookingsByDateRangeRequestHandler(IDapperRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<StaffBookingDto>> Handle(StaffBookingsByDateRangeRequest request, CancellationToken cancellationToken)
    {
        var staffBookings = await _repository.QueryAsync<StaffBookingDto>(
            sql: "GetStaffBookingsReport",
            param: new
            {
                request.TenantId,
                request.FromDate,
                request.ToDate,
                Search = request.Keyword,
                request.PageNumber,
                request.PageSize,
                OrderBy = request.OrderBy is {Length: > 0} ? request.OrderBy.First() : "FullName Ascending"
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);
        
        if (staffBookings.Any())
            return new PaginationResponse<StaffBookingDto>(staffBookings.ToList(), staffBookings.First().TotalCount,
                request.PageNumber, request.PageSize);
            
        return new PaginationResponse<StaffBookingDto>(staffBookings.ToList(), 0, request.PageNumber, request.PageSize);
    }
}