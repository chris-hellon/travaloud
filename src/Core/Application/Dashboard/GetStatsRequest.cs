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
    private readonly IUserService _userService;
    private readonly IRepositoryFactory<Property> _propertiesRepo;
    private readonly IRepositoryFactory<Booking> _bookingsRepo;
    private readonly IRepositoryFactory<BookingItem> _bookingItemsRepo;
    private readonly IRepositoryFactory<Tour> _toursRepo;
    private readonly IStringLocalizer<GetStatsRequestHandler> _localizer;
    public GetStatsRequestHandler(IUserService userService, IRepositoryFactory<Property> propertiesRepo,
        IRepositoryFactory<Booking> bookingsRepo, IRepositoryFactory<Tour> toursRepo,
        IStringLocalizer<GetStatsRequestHandler> localizer, IRepositoryFactory<BookingItem> bookingItemsRepo)
    {
        _userService = userService;
        _propertiesRepo = propertiesRepo;
        _bookingsRepo = bookingsRepo;
        _toursRepo = toursRepo;
        _localizer = localizer;
        _bookingItemsRepo = bookingItemsRepo;
    }

    public async Task<StatsDto> Handle(GetStatsRequest request, CancellationToken cancellationToken)
    {
        var tourBookings = await _bookingsRepo.ListAsync(new TourBookingsCountSpec(), cancellationToken);
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
            Revenue = group.Key.PaidItems.Sum(x => x.TotalAmount)
        });

        var stats = new StatsDto()
        {
            PropertiesCount = await _propertiesRepo.CountAsync(cancellationToken),
            BookingsCount = await _bookingsRepo.CountAsync(cancellationToken),
            TourBookingsCount = tourItemBookings.Sum(x => x.AllItemsCount),
            TourBookingsRevenue = tourItemBookings.Sum(x => x.Revenue),
            PropertyBookingsCount = await _bookingsRepo.CountAsync(new PropertyBookingsCountSpec(), cancellationToken),
            GuestsCount = request.Guests.Count,
            ToursCount = await _toursRepo.CountAsync(cancellationToken),
        };

        var selectedYear = DateTime.Now.Year;
        var propertiesFigure = new double[13];
        var bookingsFigure = new double[13];
        var toursFigure = new double[13];
        for (var i = 1; i <= 12; i++)
        {
            var month = i;
            var filterStartDate = new DateTime(selectedYear, month, 01);
            var filterEndDate =
                new DateTime(selectedYear, month, DateTime.DaysInMonth(selectedYear, month), 23, 59,
                    59); // Monthly Based

            var propertiesSpec = new AuditableEntitiesByCreatedOnBetweenSpec<Property>(filterStartDate, filterEndDate);
            var bookingsSpec = new AuditableEntitiesByCreatedOnBetweenSpec<Booking>(filterStartDate, filterEndDate);
            var toursSpec = new AuditableEntitiesByCreatedOnBetweenSpec<Tour>(filterStartDate, filterEndDate);

            propertiesFigure[i - 1] = await _propertiesRepo.CountAsync(propertiesSpec, cancellationToken);
            bookingsFigure[i - 1] = await _bookingsRepo.CountAsync(bookingsSpec, cancellationToken);
            toursFigure[i - 1] = await _toursRepo.CountAsync(toursSpec, cancellationToken);
        }

        stats.DataEnterBarChart.Add(new ChartSeries {Name = _localizer["Bookings"], Data = bookingsFigure});

        return stats;
    }
}