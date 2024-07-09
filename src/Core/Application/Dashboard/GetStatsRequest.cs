using System.Data;
using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Dashboard;

public class GetStatsRequest : IRequest<StatsDto>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string TenantId { get; set; }

    public GetStatsRequest(string tenantId)
    {
        TenantId = tenantId;
    }
}

public class GetStatsRequestHandler : IRequestHandler<GetStatsRequest, StatsDto>
{
    private readonly IRepositoryFactory<Booking> _bookingsRepo;
    private readonly IRepositoryFactory<Tour> _toursRepo;
    private readonly IDapperRepository _dapperRepository;

    public GetStatsRequestHandler(
        IRepositoryFactory<Booking> bookingsRepo, 
        IRepositoryFactory<Tour> toursRepo, IDapperRepository dapperRepository)
    {
        _bookingsRepo = bookingsRepo;
        _toursRepo = toursRepo;
        _dapperRepository = dapperRepository;
    }

    public async Task<StatsDto> Handle(GetStatsRequest request, CancellationToken cancellationToken)
    {
        return await _dapperRepository.QuerySingleAsync<StatsDto>("GetDashboard", new
        {
            TenantId = request.TenantId
        }, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
        
        // var tourBookingsRequest = Task.Run(() => _bookingsRepo.ListAsync(new TourBookingsCountSpec(null, null), cancellationToken), cancellationToken);
        // var toursRequest = Task.Run(() => _toursRepo.ListAsync(cancellationToken), cancellationToken);
        //
        // await Task.WhenAll(
        //     toursRequest,
        //     tourBookingsRequest);
        //
        // var tourBookings = tourBookingsRequest.Result;
        // var tourItemBookings = tourBookings.GroupBy(x => new
        //     {
        //         Booking = x,
        //         PaidItems = x.Items.Where(i => i.TourId.HasValue && x.IsPaid),
        //         AllItems = x.Items.Where(i => i.TourId.HasValue)
        //     })
        //     .Select(group => new
        //     {
        //         GroupKey = group.Key,
        //         PaidItemCount = group.Key.PaidItems.Count(),
        //         AllItemsCount = group.Key.AllItems.Count(),
        //         Revenue = group.Key.PaidItems.Sum(x => x.TotalAmount),
        //         PaidItems = group.Key.PaidItems,
        //         AllItems = group.Key.AllItems
        //     });
        //
        // var tours = toursRequest.Result;
        //
        // var stats = new StatsDto()
        // {
        //     TourBookingsCount = tourItemBookings.Sum(x => x.AllItemsCount),
        //     TourBookingsRevenue = tourItemBookings.Sum(x => x.Revenue),
        //     PaidTourBookings = tourItemBookings.SelectMany(x => x.PaidItems),
        //     AllTourBookings = tourItemBookings.SelectMany(x => x.AllItems)
        // };
        //
        // foreach (var tour in tours)
        // {
        //     var summary = new TourBookingsBarChartSummary()
        //     {
        //         TourName = tour.Name,
        //         MonthlyAmounts = []
        //     };
        //     
        //     var selectedYear = DateTime.Now.Year;
        //     for (var i = 1; i <= 12; i++)
        //     {
        //         var month = i;
        //         var filterStartDate = new DateTime(selectedYear, month, 01);
        //         var filterEndDate = new DateTime(selectedYear, month, DateTime.DaysInMonth(selectedYear, month), 23, 59, 59); // Monthly Based
        //
        //         var bookedTours = stats.AllTourBookings.Where(x => x.TourId == tour.Id &&  x.CreatedOn.Date >= filterStartDate.Date && x.CreatedOn <= filterEndDate.Date);
        //         
        //         if (bookedTours.Any())
        //             summary.MonthlyAmounts.Add(new TourBookingsBarChartSummary.MonthAmount()
        //             {
        //                 Amount = bookedTours.Sum(x => x.TotalAmount),
        //                 Count = bookedTours.Count(),
        //                 MonthYear = filterStartDate
        //             });
        //         else
        //         {
        //             summary.MonthlyAmounts.Add(new TourBookingsBarChartSummary.MonthAmount()
        //             {
        //                 Amount = 0,
        //                 Count = 0,
        //                 MonthYear = filterStartDate
        //             });
        //         }
        //     }
        //     
        //     stats.TourBookingsBarChartSummaries.Add(summary);
        // }
    }
}