using MediatR;
using Travaloud.Application.BackgroundJobs.Commands;
using Travaloud.Application.Catalog.Destinations.Queries;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Application.Common.Interfaces;
using Travaloud.Application.Common.Persistence;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Infrastructure.BackgroundJobs;

public class SendDailyTourManifestBatchHandler : IRequestHandler<SendDailyTourManifestBatch>
{
    private readonly IDestinationsService _destinationsService;
    private readonly IToursService _toursService;
    private readonly IJobService _jobService;
    private readonly IRepositoryFactory<TourDate> _tourDateRepository;
    
    public SendDailyTourManifestBatchHandler(
        IDestinationsService destinationsService,
        IToursService toursService,
        IRepositoryFactory<TourDate> tourDateRepository, IJobService jobService)
    {
        _destinationsService = destinationsService;
        _toursService = toursService;
        _tourDateRepository = tourDateRepository;
        _jobService = jobService;
    }

    public async Task Handle(SendDailyTourManifestBatch request, CancellationToken cancellationToken)
    {
        var destinations = await _destinationsService.SearchAsync(new SearchDestinationsRequest()
        {
            PageNumber = 1,
            PageSize = int.MaxValue
        });

        if (destinations == null || destinations.Data.Count == 0) return;
        
        foreach (var destination in destinations.Data)
        {
            var tours = await _toursService.GetToursByDestinations(
                new GetToursByDestinationsRequest(new List<DefaultIdType>() { destination.Id }));

            var tourWithoutDatesDtos = tours as TourWithoutDatesDto[] ?? tours.ToArray();
            if (tourWithoutDatesDtos.Length == 0) continue;
                
            foreach (var tour in tourWithoutDatesDtos)
            {
                var tourDates =
                    await _tourDateRepository.ListAsync(new TourDatesByTourIdNoLimitSpec(tour.Id),
                        cancellationToken);

                tourDates = tourDates.Where(x => x.StartDate.Date == DateTime.Now.Date).ToList();

                if (tourDates.Count == 0) continue;
                
                foreach (var tourDate in tourDates)
                { 
                    var jobId = $"{destination.Name}, {tour.Name} {tourDate.StartDate.TimeOfDay} Daily Tour Manifest";
                        
                    var cronExpression = $"{tourDate.StartDate.Minute} {tourDate.StartDate.AddHours(-1).Hour} * * *";
                    _jobService.AddOrUpdate(jobId,
                        new SendDailyTourManifest(tour.Id,
                            tourDate.Id,
                            destination.Id),
                        cronExpression,
                        TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
                }
            }
        }
    }
}