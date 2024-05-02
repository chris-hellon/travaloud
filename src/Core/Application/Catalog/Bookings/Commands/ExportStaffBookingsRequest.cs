using System.Data;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Common.Exporters;

namespace Travaloud.Application.Catalog.Bookings.Commands;

public class ExportStaffBookingsRequest: BaseFilter, IRequest<Stream>
{
    public string TenantId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}

internal class StaffBookingsByDateRangeRequestHandler : IRequestHandler<ExportStaffBookingsRequest, Stream>
{
    private readonly IDapperRepository _repository;
    private readonly IExcelWriter _excelWriter;
    
    public StaffBookingsByDateRangeRequestHandler(IDapperRepository repository, IExcelWriter excelWriter)
    {
        _repository = repository;
        _excelWriter = excelWriter;
    }
    
    public async Task<Stream> Handle(ExportStaffBookingsRequest request, CancellationToken cancellationToken)
    {
        var staffBookings = await _repository.QueryAsync<StaffBookingDto>(
            sql: "GetStaffBookingsReport",
            param: new
            {
                request.TenantId,
                request.FromDate,
                request.ToDate
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);
        
        return _excelWriter.WriteToStream(staffBookings.ToList());
    }
}