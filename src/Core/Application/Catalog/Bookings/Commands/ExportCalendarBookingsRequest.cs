using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Common.Exporters;
using Travaloud.Application.Dashboard;
using Travaloud.Application.Identity.Users;

namespace Travaloud.Application.Catalog.Bookings.Commands;

public class ExportCalendarBookingsRequest : IRequest<Stream>
{
    public GetBookingItemsByDateRequest Request { get; set; }

    public ExportCalendarBookingsRequest(GetBookingItemsByDateRequest request)
    {
        Request = request;
    }
}

public class ExportCalendarBookingsRequestHandler : IRequestHandler<ExportCalendarBookingsRequest, Stream>
{
    private readonly IExcelWriter _excelWriter;
    private readonly IUserService _userService;
    private readonly IDashboardService _dashboardService;
    
    public ExportCalendarBookingsRequestHandler(
        IExcelWriter excelWriter, 
        IUserService userService, IDashboardService dashboardService)
    {
        _excelWriter = excelWriter;
        _userService = userService;
        _dashboardService = dashboardService;
    }

    public async Task<Stream> Handle(ExportCalendarBookingsRequest request, CancellationToken cancellationToken)
    {
        var bookingExportsRequest = await _dashboardService.GetTourBookingItemsByDateAsync(
            request.Request);

        var bookingExports = bookingExportsRequest.Data;
        
        
        var staffIds = bookingExports.Select(x => x.CreatedBy.ToString()).ToList();

        var staff = await _userService.SearchAsync(staffIds, CancellationToken.None);

        if (staff.Count == 0) return _excelWriter.WriteToStream(bookingExports);
        {
            var bookings = bookingExports.Select(x =>
            {
                var staffMember = staff.FirstOrDefault(s => s.Id == x.CreatedBy);

                x.BookingStaffName = staffMember == null || staffMember.Id.ToString() == x.BookingGuestId ? "Direct from Website"  : $"{staffMember.FirstName} {staffMember.LastName}";
                
                return x;
            });

            bookingExports = bookings.ToList();
        }

        return _excelWriter.WriteToStream(bookingExports);
    }
}