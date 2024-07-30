using Travaloud.Application.Catalog.Destinations.Queries;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.BackgroundJobs.Commands;

// public class SendDailyTourManifestRequest : IRequest<List<string>>
// {
//     
// }

public class SendDailyTourManifest : IRequest
{
    public DefaultIdType TourId { get; set; }
    public DefaultIdType TourDateId { get; set; }
    public DefaultIdType DestinationId { get; set; }
    public string TourName { get; set; }
    public DateTime TourStartDate { get; set; }
    public string TenantId { get; set; }
    public string TenantName { get; set; }
    public string TenantLogoImageUrl { get; set; }
    public string TenantBackgroundColor { get; set; }

    public SendDailyTourManifest(DefaultIdType tourId,
        DefaultIdType tourDateId,
        DefaultIdType destinationId,
        string tenantId,
        string tourName,
        DateTime tourStartDate, string tenantName, string tenantLogoImageUrl, string tenantBackgroundColor)
    {
        TourId = tourId;
        TourDateId = tourDateId;
        DestinationId = destinationId;
        TenantId = tenantId;
        TourName = tourName;
        TourStartDate = tourStartDate;
        TenantName = tenantName;
        TenantLogoImageUrl = tenantLogoImageUrl;
        TenantBackgroundColor = tenantBackgroundColor;
    }
}

// internal class SendDailyTourManifestRequestHandler : IRequestHandler<SendDailyTourManifestRequest, List<string>>
// {
//     private readonly IJobService _jobService;
//     private readonly IDestinationsService _destinationsService;
//     private readonly IToursService _toursService;
//     private readonly IRepositoryFactory<TourDate> _tourDateRepository;
//     
//     public SendDailyTourManifestRequestHandler(
//         IJobService jobService, 
//         IDestinationsService destinationsService, 
//         IToursService toursService, 
//         IRepositoryFactory<TourDate> tourDateRepository)
//     {
//         _jobService = jobService;
//         _destinationsService = destinationsService;
//         _toursService = toursService;
//         _tourDateRepository = tourDateRepository;
//     }
//
//     public async Task<List<string>> Handle(SendDailyTourManifestRequest request, CancellationToken cancellationToken)
//     {
//         var jobIds = new List<string>();
//
//         var destinations = await _destinationsService.SearchAsync(new SearchDestinationsRequest()
//         {
//             PageNumber = 1,
//             PageSize = int.MaxValue
//         });
//
//         if (destinations == null || destinations.Data.Count == 0) return jobIds;
//         
//         foreach (var destination in destinations.Data)
//         {
//             var tours = await _toursService.GetToursByDestinations(
//                 new GetToursByDestinationsRequest(new List<DefaultIdType>() { destination.Id }));
//
//             var tourWithoutDatesDtos = tours as TourWithoutDatesDto[] ?? tours.ToArray();
//             if (tourWithoutDatesDtos.Length == 0) continue;
//                 
//             foreach (var tour in tourWithoutDatesDtos)
//             {
//                 var tourDates =
//                     await _tourDateRepository.ListAsync(new TourDatesByTourIdNoLimitSpec(tour.Id),
//                         cancellationToken);
//
//                 tourDates = tourDates.Where(x => x.StartDate.Date == DateTime.Now.Date).ToList();
//
//                 if (tourDates.Count == 0) continue;
//                 
//                 foreach (var tourDate in tourDates)
//                 {
//                     var jobId = $"{destination.Name}, {tour.Name} Daily Tour Manifest";
//                         
//                     var cronExpression = $"{tourDate.StartDate.Minute} {tourDate.StartDate.AddHours(-1).Hour} * * *";
//                     _jobService.AddOrUpdate(jobId,
//                         new SendDailyTourManifest(tour.Id,
//                             tourDate.Id,
//                             destination.Id),
//                         cronExpression,
//                         TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
//
//                     jobIds.Add(jobId);
//                 }
//             }
//         }
//
//         return jobIds;
//     }
// }