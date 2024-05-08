using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Application.Identity.Users;
using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Domain.Catalog.Properties;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Dashboard;

public class GetStatsRequest : IRequest<StatsDto>
{
    public List<UserDetailsDto> Guests { get; set; }
}

public class GetStatsRequestHandler : IRequestHandler<GetStatsRequest, StatsDto>
{
    private readonly IRepositoryFactory<Property> _propertiesRepo;
    private readonly IRepositoryFactory<Booking> _bookingsRepo;
    private readonly IRepositoryFactory<Tour> _toursRepo;

    public GetStatsRequestHandler(
        IRepositoryFactory<Property> propertiesRepo,
        IRepositoryFactory<Booking> bookingsRepo, 
        IRepositoryFactory<Tour> toursRepo)
    {
        _propertiesRepo = propertiesRepo;
        _bookingsRepo = bookingsRepo;
        _toursRepo = toursRepo;
    }

    public async Task<StatsDto> Handle(GetStatsRequest request, CancellationToken cancellationToken)
    {
        var tourBookingsRequest = Task.Run(() => _bookingsRepo.ListAsync(new TourBookingsCountSpec(), cancellationToken), cancellationToken);
        var toursRequest = Task.Run(() => _toursRepo.ListAsync(cancellationToken), cancellationToken);
        var propertiesCountRequest = Task.Run(() => _propertiesRepo.CountAsync(cancellationToken), cancellationToken);
        var bookingsCountRequest = Task.Run(() => _bookingsRepo.CountAsync(cancellationToken), cancellationToken);
        var propertyBookingsCountRequest = Task.Run(() => _bookingsRepo.CountAsync(new PropertyBookingsCountSpec(), cancellationToken), cancellationToken);

        await Task.WhenAll(
            tourBookingsRequest, 
            toursRequest, 
            propertiesCountRequest, 
            bookingsCountRequest,
            propertyBookingsCountRequest);
        
        var tourBookings = tourBookingsRequest.Result;
        var tourItemBookings = tourBookings.GroupBy(x => new
            {
                Booking = x,
                PaidItems = x.Items.Where(i => i.TourId.HasValue && x.IsPaid),
                AllItems = x.Items.Where(i => i.TourId.HasValue)
            })
            .Select(group => new
            {
                GroupKey = group.Key,
                PaidItemCount = group.Key.PaidItems.Count(),
                AllItemsCount = group.Key.AllItems.Count(),
                Revenue = group.Key.PaidItems.Sum(x => x.TotalAmount),
                PaidItems = group.Key.PaidItems,
                AllItems = group.Key.AllItems
            });

        var tours = toursRequest.Result;
        
        var stats = new StatsDto()
        {
            PropertiesCount = propertiesCountRequest.Result,
            BookingsCount = bookingsCountRequest.Result,
            TourBookingsCount = tourItemBookings.Sum(x => x.AllItemsCount),
            TourBookingsRevenue = tourItemBookings.Sum(x => x.Revenue),
            PropertyBookingsCount = bookingsCountRequest.Result,
            GuestsCount = request.Guests.Count,
            ToursCount = tours.Count,
            PaidTourBookings = tourItemBookings.SelectMany(x => x.PaidItems),
            AllTourBookings = tourItemBookings.SelectMany(x => x.AllItems)
        };

        foreach (var tour in tours)
        {
            var summary = new TourBookingsBarChartSummary()
            {
                TourName = tour.Name,
                MonthlyAmounts = new List<TourBookingsBarChartSummary.MonthAmount>()
            };
            
            var selectedYear = DateTime.Now.Year;
            for (var i = 1; i <= 12; i++)
            {
                var month = i;
                var filterStartDate = new DateTime(selectedYear, month, 01);
                var filterEndDate = new DateTime(selectedYear, month, DateTime.DaysInMonth(selectedYear, month), 23, 59, 59); // Monthly Based

                var bookedTours = stats.AllTourBookings.Where(x => x.TourId == tour.Id &&  x.CreatedOn.Date >= filterStartDate.Date && x.CreatedOn <= filterEndDate.Date);
                
                if (bookedTours.Any())
                    summary.MonthlyAmounts.Add(new TourBookingsBarChartSummary.MonthAmount()
                    {
                        Amount = bookedTours.Sum(x => x.TotalAmount),
                        Count = bookedTours.Count(),
                        MonthYear = filterStartDate
                    });
                else
                {
                    summary.MonthlyAmounts.Add(new TourBookingsBarChartSummary.MonthAmount()
                    {
                        Amount = 0,
                        Count = 0,
                        MonthYear = filterStartDate
                    });
                }
            }
            
            stats.TourBookingsBarChartSummaries.Add(summary);
        }

        return stats;
    }
}