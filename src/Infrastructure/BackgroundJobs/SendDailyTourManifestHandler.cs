using Finbuckle.MultiTenant;
using MediatR;
using Microsoft.Extensions.Options;
using Travaloud.Application.BackgroundJobs.Commands;
using Travaloud.Application.Catalog.Destinations.Specification;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Properties.Queries;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Application.Common.Mailing;
using Travaloud.Application.Common.Persistence;
using Travaloud.Application.Dashboard;
using Travaloud.Application.Identity.Users;
using Travaloud.Domain.Catalog.Destinations;
using Travaloud.Domain.Catalog.Tours;
using Travaloud.Infrastructure.Mailing;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Shared.Authorization;

namespace Travaloud.Infrastructure.BackgroundJobs;

public class SendDailyTourManifestHandler : IRequestHandler<SendDailyTourManifest>
{
    private readonly IDestinationsService _destinationsService;
    private readonly IToursService _toursService;
    private readonly IDashboardService _dashboardService;
    private readonly IUserService _userService;
    private readonly IMultiTenantContextAccessor<TravaloudTenantInfo> _multiTenantContextAccessor;
    private readonly IExcelWriter _excelWriter;
    private readonly IPropertiesService _propertiesService;
    private readonly IMailService _mailService;
    private readonly MailSettings _mailSettings;


    private readonly IRepositoryFactory<Tour> _tourRepository;
    private readonly IRepositoryFactory<TourDate> _tourDateRepository;
    private readonly IRepositoryFactory<Destination> _destinationRepository;
    
    public SendDailyTourManifestHandler(IDestinationsService destinationsService,
        IToursService toursService,
        IDashboardService dashboardService,
        IUserService userService,
        IMultiTenantContextAccessor<TravaloudTenantInfo> multiTenantContextAccessor,
        IExcelWriter excelWriter,
        IPropertiesService propertiesService,
        IOptions<MailSettings> mailSettings, IMailService mailService, 
        IRepositoryFactory<TourDate> tourDateRepository, 
        IRepositoryFactory<Destination> destinationRepository, IRepositoryFactory<Tour> tourRepository)
    {
        _destinationsService = destinationsService;
        _toursService = toursService;
        _dashboardService = dashboardService;
        _userService = userService;
        _multiTenantContextAccessor = multiTenantContextAccessor;
        _excelWriter = excelWriter;
        _propertiesService = propertiesService;
        _mailService = mailService;
        _tourDateRepository = tourDateRepository;
        _destinationRepository = destinationRepository;
        _tourRepository = tourRepository;
        _mailSettings = mailSettings.Value;
    }

    public async Task Handle(SendDailyTourManifest request, CancellationToken cancellationToken)
    {
        var tourDate = await _tourDateRepository.FirstOrDefaultAsync(new TourDateByIdSpec(request.TourDateId), cancellationToken);

        if (tourDate == null)
            return;

        var destination = await _destinationRepository.FirstOrDefaultAsync(new DestinationByIdWithToursSpec(request.DestinationId), cancellationToken);

        if (destination == null)
            return;

        var tour = await _tourRepository.FirstOrDefaultAsync(new TourByIdSpec(request.TourId), cancellationToken);

        if (tour == null)
            return;

        var tenantId = _multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Id ??
                       throw new Exception("No Tenant found.");
        
        var guests = await _userService.SearchByDapperAsync(new SearchByDapperRequest()
        {
            Role = TravaloudRoles.Guest,
            PageNumber = 1,
            PageSize = 99999,
            TenantId = tenantId
        }, cancellationToken);

        if (guests.Data.Count == 0) return;
        
        var adaptedFilter = new GetBookingItemsByDateRequest
        {
            Guests = guests.Data,
            TourDateId = request.TourDateId,
            TourId = request.TourId
        };

        var destinationManifests = new Dictionary<string, byte[]>();
        
        var todaysTours = await _dashboardService.GetTourBookingItemsByDateAsync(adaptedFilter);

        if (todaysTours.Data.Count != 0)
        {
            var fileStream = _excelWriter.WriteToStream(todaysTours.Data);

            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream, cancellationToken);
        
            destinationManifests.Add($"{tour.Name} {tourDate.StartDate:hh:mm tt}.xlsx", memoryStream.ToArray());
        
            var properties = await _propertiesService.GetPropertiesByDestination(new GetPropertiesByDestinationRequest(tenantId, destination.Id));

            foreach (var property in properties)
            {
                if (string.IsNullOrEmpty(property.EmailAddress)) continue;
                
                var mailRequest = new MailRequest(
                    to: [property.EmailAddress],
                   
                    subject: $"Today's Tour Manifests for {property.Name}",
                    body: $"Please find attached Today's Tour Manifests for {property.Name}.",
                    bcc: (_mailSettings.BccAddress != null ? _mailSettings.BccAddress.ToList() : ["admin@travaloud.com"])!,
                    attachmentData: destinationManifests);
                        
                await _mailService.SendAsync(mailRequest);
            }
        }
    }
}