using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Common.Exporters;
using Travaloud.Application.Identity.Users;

namespace Travaloud.Application.Catalog.Bookings.Commands;

public class ExportCalendarBookingsRequest : IRequest<Stream>
{
    public List<BookingExportDto> Exports { get; set; }

    public ExportCalendarBookingsRequest(List<BookingExportDto> exports)
    {
        Exports = exports;
    }
}

public class ExportCalendarBookingsRequestHandler : IRequestHandler<ExportCalendarBookingsRequest, Stream>
{
    private readonly IExcelWriter _excelWriter;
    private readonly IUserService _userService;
    
    public ExportCalendarBookingsRequestHandler(
        IExcelWriter excelWriter, 
        IUserService userService)
    {
        _excelWriter = excelWriter;
        _userService = userService;
    }

    public async Task<Stream> Handle(ExportCalendarBookingsRequest request, CancellationToken cancellationToken)
    {
        var staffIds = request.Exports.Select(x => x.CreatedBy.ToString()).ToList();

        var staff = await _userService.SearchAsync(staffIds, CancellationToken.None);

        if (staff.Count == 0) return _excelWriter.WriteToStream(request.Exports);
        {
            var bookings = request.Exports.Select(x =>
            {
                var staffMember = staff.FirstOrDefault(s => s.Id == x.CreatedBy);

                x.BookingStaffName = staffMember == null || staffMember.Id.ToString() == x.BookingGuestId ? "Direct from Website"  : $"{staffMember.FirstName} {staffMember.LastName}";
                
                return x;
            });

            request.Exports = bookings.ToList();
        }

        return _excelWriter.WriteToStream(request.Exports);
    }
}