using System.Data;
using Dapper;
using Travaloud.Application.Identity.Users;
using Travaloud.Shared.Authorization;

namespace Travaloud.Application.Dashboard;

public class GetCalendarRequest : IRequest<IList<CalendarDto>>
{
    public string TenantId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public IEnumerable<DefaultIdType>? TourIds { get; set; }
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
    private readonly ICurrentUser _currentUser;
    private readonly IUserService _userService;

    public GetCalendarRequestHandler(IDapperRepository dapperRepository, ICurrentUser currentUser, IUserService userService)
    {
        _dapperRepository = dapperRepository;
        _currentUser = currentUser;
        _userService = userService;
    }

    public async Task<IList<CalendarDto>> Handle(GetCalendarRequest request, CancellationToken cancellationToken)
    {
        var tourStartDate = request.FromDate.Date + new TimeSpan(00, 00, 00);
        var tourEndDate = request.ToDate.Date + new TimeSpan(23, 59, 59);
        
        var isSupplier = _currentUser.IsInRole(TravaloudRoles.Supplier);

        if (isSupplier)
        {
            var userClaims = await _userService.GetUserClaims(new GetUserClaimsRequest()
            {
                ClaimType = "SupplierTour",
                UserId = _currentUser.GetUserId().ToString()
            }, CancellationToken.None);

            request.TourIds = userClaims.Select(x => DefaultIdType.Parse(x.ClaimValue)).ToArray();
        }
        
        var tourIdsDt = new DataTable();
        tourIdsDt.Columns.Add("Id");

        if (request.TourIds != null)
        {
            foreach (var tourId in request.TourIds)
            {
                var dataRow = tourIdsDt.NewRow();
                dataRow["Id"] = tourId;

                tourIdsDt.Rows.Add(dataRow);
            }
        }
        
        var calendarItems = await _dapperRepository.QueryAsync<CalendarDto>("GetCalendar", new
        {
            FromDate = tourStartDate,
            ToDate = tourEndDate,
            request.TenantId,
            TourIds = tourIdsDt.AsTableValuedParameter(),
        }, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);

        return calendarItems.ToList();
    }
}