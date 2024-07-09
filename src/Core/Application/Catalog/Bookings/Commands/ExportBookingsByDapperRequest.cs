using System.Data;
using Dapper;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Common.Exporters;

namespace Travaloud.Application.Catalog.Bookings.Commands;

public class ExportBookingsByDapperRequest : IRequest<Stream>
{
    public required string TenantId { get; set; }
    public string? Description { get; set; }
    public DateTime? BookingStartDate { get; set; }
    public DateTime? BookingEndDate { get; set; }
    public DateTime? TourStartDate { get; set; }
    public DateTime? TourEndDate { get; set; }
    public DefaultIdType? TourId { get; set; }
    public DefaultIdType? TourDateId { get; set; }
    public IEnumerable<DefaultIdType>? TourIds { get; set; }
    public bool HideRefunded { get; set; } = true;
}

internal class ExportBookingsByDapperRequestHandler : IRequestHandler<ExportBookingsByDapperRequest, Stream>
{
    private readonly IDapperRepository _repository;
    private readonly IExcelWriter _excelWriter;

    public ExportBookingsByDapperRequestHandler(
        IDapperRepository repository, 
        IExcelWriter excelWriter)
    {
        _repository = repository;
        _excelWriter = excelWriter;
    }
    
    public async Task<Stream> Handle(ExportBookingsByDapperRequest request, CancellationToken cancellationToken)
    {
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

        var bookingExports = await _repository.QueryAsync<BookingExportDto>("GetBookingsExports", new
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
        
        return _excelWriter.WriteToStream(bookingExports.ToList());
    }
}