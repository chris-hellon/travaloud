using System.Data;
using Travaloud.Application.Catalog.Bookings.Dto;

namespace Travaloud.Application.Catalog.Bookings.Queries;

public class StaffBookingsByDateRangeRequest : BaseFilter, IRequest<IEnumerable<StaffBookingDto>>
{
    public string TenantId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}

internal class StaffBookingsByDateRangeRequestHandler : IRequestHandler<StaffBookingsByDateRangeRequest, IEnumerable<StaffBookingDto>>
{
    private readonly IDapperRepository _repository;

    public StaffBookingsByDateRangeRequestHandler(IDapperRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<StaffBookingDto>> Handle(StaffBookingsByDateRangeRequest request, CancellationToken cancellationToken)
    {
        return await _repository.QueryAsync<StaffBookingDto>(
            sql: "GetStaffBookingsReport",
            param: new
            {
                request.TenantId,
                request.FromDate,
                request.ToDate
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);
    }
}