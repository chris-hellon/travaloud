using Travaloud.Application.Identity.Users;
using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Domain.Catalog.Properties;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Dashboard;

public class GetStatsRequest : IRequest<StatsDto>
{
}

public class GetStatsRequestHandler : IRequestHandler<GetStatsRequest, StatsDto>
{
    private readonly IUserService _userService;
    private readonly IRepositoryFactory<Property> _propertiesRepo;
    private readonly IRepositoryFactory<Booking> _bookingsRepo;
    private readonly IRepositoryFactory<Tour> _toursRepo;
    private readonly IStringLocalizer<GetStatsRequestHandler> _localizer;

    public GetStatsRequestHandler(IUserService userService, IRepositoryFactory<Property> propertiesRepo,
        IRepositoryFactory<Booking> bookingsRepo, IRepositoryFactory<Tour> toursRepo,
        IStringLocalizer<GetStatsRequestHandler> localizer)
    {
        _userService = userService;
        _propertiesRepo = propertiesRepo;
        _bookingsRepo = bookingsRepo;
        _toursRepo = toursRepo;
        _localizer = localizer;
    }

    public async Task<StatsDto> Handle(GetStatsRequest request, CancellationToken cancellationToken)
    {
        var stats = new StatsDto()
        {
            PropertiesCount = await _propertiesRepo.CountAsync(cancellationToken),
            BookingsCount = await _bookingsRepo.CountAsync(cancellationToken),
            GuestsCount = await _userService.GetCountAsync(cancellationToken, "Guest"),
            ToursCount = await _toursRepo.CountAsync(cancellationToken)
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