using System.Data;

namespace Travaloud.Application.Dashboard;

public class GetCalendarRequest : IRequest<IList<CalendarDto>>
{
    public string TenantId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }

    public GetCalendarRequest(string tenantId, DateTime fromDate, DateTime toDate)
    {
        TenantId = tenantId;
        FromDate = fromDate;
        ToDate = toDate;
    }
}

internal class GetCalendarRequestHandler : IRequestHandler<GetCalendarRequest, IList<CalendarDto>>
{
    private readonly IDapperRepository _dapperRepository;

    public GetCalendarRequestHandler(IDapperRepository dapperRepository)
    {
        _dapperRepository = dapperRepository;
    }

    public async Task<IList<CalendarDto>> Handle(GetCalendarRequest request, CancellationToken cancellationToken)
    {
        var tourStartDate = request.FromDate.Date + new TimeSpan(00, 00, 00);
        var tourEndDate = request.ToDate.Date + new TimeSpan(23, 59, 59);
        
        var calendarItems = await _dapperRepository.QueryAsync<CalendarDto>("GetCalendar", new
        {
            FromDate = tourStartDate,
            ToDate = tourEndDate,
            request.TenantId
        }, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);

        return calendarItems.ToList();
    }
}