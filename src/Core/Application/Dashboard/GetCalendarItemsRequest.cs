using System.Data;
using Dapper;
using Travaloud.Application.Identity.Users;
using Travaloud.Shared.Authorization;

namespace Travaloud.Application.Dashboard;

public class GetCalendarItemsRequest : IRequest<IEnumerable<CalendarItemDto>>
{
    public required string TenantId { get; set; }
    public DateTime? TourStartDate { get; set; }
    public DateTime? TourEndDate { get; set; }
    public TimeSpan? TourStartTime { get; set; }
    public DefaultIdType? TourId { get; set; }
    public IEnumerable<DefaultIdType>? TourIds { get; set; }
    public bool PaidOnly { get; set; }
}

internal class GetCalendarItemsRequestHandler : IRequestHandler<GetCalendarItemsRequest, IEnumerable<CalendarItemDto>>
{
    private readonly IUserService _userService;
    private readonly ICurrentUser _currentUser;
    private readonly IDapperRepository _dapperRepository;
    
    public GetCalendarItemsRequestHandler(
        IUserService userService, 
        ICurrentUser currentUser,
        IDapperRepository dapperRepository)
    {
        _userService = userService;
        _currentUser = currentUser;
        _dapperRepository = dapperRepository;
    }

    public async Task< IEnumerable<CalendarItemDto>> Handle(GetCalendarItemsRequest request, CancellationToken cancellationToken)
    {
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

        var tourStartDate = request.TourStartDate.HasValue
            ? request.TourStartDate.Value.Date + (request.TourStartTime ?? new TimeSpan(00, 00, 00))
            : new DateTime?();

        var tourEndDate = request.TourEndDate.HasValue
            ? request.TourEndDate.Value.Date + (request.TourStartTime ?? new TimeSpan(23, 59, 59))
            : new DateTime?();
        
        return await _dapperRepository.QueryAsync<CalendarItemDto>("GetCalendarItems", new
        {
            TourStartDate = tourStartDate,
            TourEndDate = tourEndDate,
            request.TourId,
            TourIds = tourIdsDt.AsTableValuedParameter(),
            request.TenantId,
            request.PaidOnly
        }, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
    }
}