using System.Data;
using Dapper;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Identity.Users;
using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Shared.Authorization;

namespace Travaloud.Application.Dashboard;

public class GetBookingItemsByDateRequest : PaginationFilter, IRequest<PaginationResponse<BookingExportDto>>
{
    public required string TenantId { get; set; }
    public DateTime? TourStartDate { get; set; }
    public DateTime? TourEndDate { get; set; }
    public DateTime? BookingStartDate { get; set; }
    public DateTime? BookingEndDate { get; set; }
    public DefaultIdType? TourId { get; set; }
    public DefaultIdType? TourDateId { get; set; }
    public IEnumerable<DefaultIdType>? TourIds { get; set; }
    public string? Description { get; set; }
    public bool HideRefunded { get; set; } = true;
}

internal class GetBookingItemsByDateRequestHandler : IRequestHandler<GetBookingItemsByDateRequest, PaginationResponse<BookingExportDto>>
{
    private readonly IRepositoryFactory<BookingItem> _repository;
    private readonly IUserService _userService;
    private readonly ICurrentUser _currentUser;
    private IDapperRepository _dapperRepository;
    
    public GetBookingItemsByDateRequestHandler(
        IRepositoryFactory<BookingItem> repository, 
        IUserService userService, 
        ICurrentUser currentUser,
        IDapperRepository dapperRepository)
    {
        _repository = repository;
        _userService = userService;
        _currentUser = currentUser;
        _dapperRepository = dapperRepository;
    }

    public async Task<PaginationResponse<BookingExportDto>> Handle(GetBookingItemsByDateRequest request, CancellationToken cancellationToken)
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
            ? request.TourStartDate.Value.Date + new TimeSpan(00, 00, 00)
            : new DateTime?();

        var tourEndDate = request.TourEndDate.HasValue
            ? request.TourEndDate.Value.Date + new TimeSpan(23, 59, 59)
            : new DateTime?();
        
        var todaysTours = await _dapperRepository.QueryAsync<BookingExportDto>("GetBookingsExports", new
        {
            request.Description,
            TourStartDate = request.TourStartDate.HasValue ? request.TourStartDate.Value.Date + new TimeSpan(00, 00, 00) : new DateTime?(),
            TourEndDate = request.TourEndDate.HasValue ? request.TourEndDate.Value.Date + new TimeSpan(23, 59, 59) : new DateTime?(),
            BookingStartDate = request.BookingStartDate.HasValue ? request.BookingStartDate.Value.Date + new TimeSpan(00, 00, 00) : new DateTime?(),
            BookingEndDate = request.BookingEndDate.HasValue ? request.BookingEndDate.Value.Date + new TimeSpan(23, 59, 59) :new DateTime?(),
            request.TourId,
            request.TourDateId,
            TourIds = tourIdsDt.AsTableValuedParameter(),
            request.HideRefunded,
            request.TenantId
        }, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
        
        if (todaysTours.Count == 0)
            return new PaginationResponse<BookingExportDto>(new List<BookingExportDto>(),0,
                request.PageNumber, request.PageSize);

        var parsedTours = todaysTours.ToList();
        
        if (request.OrderBy is {Length: > 0})
        {
            parsedTours = request.OrderBy.Aggregate(parsedTours, (current, orderByField) => orderByField switch
            {
                "Reference" => current,
                "Reference Ascending" => current.OrderBy(t => t.BookingInvoiceId).ToList(),
                "Reference Descending" => current.OrderByDescending(t => t.BookingInvoiceId).ToList(),
                "GuestName" => current,
                "GuestName Ascending" => parsedTours.OrderBy(t => t.GuestName).ToList(),
                "GuestName Descending" => parsedTours.OrderByDescending(t => t.GuestName).ToList(),
                "TourName" => current,
                "TourName Ascending" => current.OrderBy(t => t.TourName).ToList(),
                "TourName Descending" => current.OrderByDescending(t => t.TourName).ToList(),
                "StartDate" => current,
                "StartDate Ascending" => current.OrderBy(t => t.StartDate).ToList(),
                "StartDate Descending" => current.OrderByDescending(t => t.StartDate).ToList(),
                "EndDate" => current,
                "EndDate Ascending" => current.OrderBy(t => t.EndDate).ToList(),
                "EndDate Descending" => current.OrderByDescending(t => t.EndDate).ToList(),
                "BookingIsPaid" => current,
                "BookingIsPaid Ascending" => current.OrderBy(t => t.BookingIsPaid).ToList(),
                "BookingIsPaid Descending" => current.OrderByDescending(t => t.BookingIsPaid).ToList(),
                "BookingWaiverSigned" => current,
                "BookingWaiverSigned Ascending" => current.OrderBy(t => t.WaiverSigned).ToList(),
                "BookingWaiverSigned Descending" => current.OrderByDescending(t => t.WaiverSigned)
                    .ToList(),
                _ => current
            });
        }

        // Apply pagination after ordering
        var totalCount = parsedTours.Count;
        var skip = (request.PageNumber - 1) * request.PageSize;
        parsedTours = parsedTours.Skip(skip).Take(request.PageSize).ToList();

        return new PaginationResponse<BookingExportDto>(parsedTours, totalCount, request.PageNumber, request.PageSize);

    }
}