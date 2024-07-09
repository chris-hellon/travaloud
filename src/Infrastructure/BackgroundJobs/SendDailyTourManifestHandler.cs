using Finbuckle.MultiTenant;
using MediatR;
using Microsoft.Extensions.Options;
using Travaloud.Application.BackgroundJobs.Commands;
using Travaloud.Application.Catalog.Destinations.Specification;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Properties.Queries;
using Travaloud.Application.Catalog.TourDates.Specification;
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
    private readonly IDashboardService _dashboardService;
    private readonly IUserService _userService;
    private readonly IMultiTenantContextAccessor<TravaloudTenantInfo> _multiTenantContextAccessor;
    private readonly IExcelWriter _excelWriter;
    private readonly IPropertiesService _propertiesService;
    private readonly IMailService _mailService;
    private readonly MailSettings _mailSettings;
    private readonly IEmailTemplateService _emailTemplateService;
    
    private readonly IRepositoryFactory<Tour> _tourRepository;
    private readonly IRepositoryFactory<TourDate> _tourDateRepository;
    private readonly IRepositoryFactory<Destination> _destinationRepository;
    
    public SendDailyTourManifestHandler(
        IDashboardService dashboardService,
        IUserService userService,
        IMultiTenantContextAccessor<TravaloudTenantInfo> multiTenantContextAccessor,
        IExcelWriter excelWriter,
        IPropertiesService propertiesService,
        IOptions<MailSettings> mailSettings, IMailService mailService, 
        IRepositoryFactory<TourDate> tourDateRepository, 
        IRepositoryFactory<Destination> destinationRepository, 
        IRepositoryFactory<Tour> tourRepository, 
        IEmailTemplateService emailTemplateService)
    {
        _dashboardService = dashboardService;
        _userService = userService;
        _multiTenantContextAccessor = multiTenantContextAccessor;
        _excelWriter = excelWriter;
        _propertiesService = propertiesService;
        _mailService = mailService;
        _tourDateRepository = tourDateRepository;
        _destinationRepository = destinationRepository;
        _tourRepository = tourRepository;
        _emailTemplateService = emailTemplateService;
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
        
        // var guests = await _userService.SearchByDapperAsync(new SearchByDapperRequest()
        // {
        //     Role = TravaloudRoles.Guest,
        //     PageNumber = 1,
        //     PageSize = 99999,
        //     TenantId = tenantId
        // }, cancellationToken);
        //
        // if (guests.Data.Count == 0) return;
        
        var adaptedFilter = new GetBookingItemsByDateRequest
        {
            //Guests = guests.Data,
            TourDateId = request.TourDateId,
            TourId = request.TourId,
            HideRefunded = true,
            TenantId = tenantId
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
                
                var tenantInfo = _multiTenantContextAccessor.MultiTenantContext.TenantInfo;

                var emailHtml = _emailTemplateService.GenerateTourManifestEmail(
                    tour.Name,
                    tenantInfo.PrimaryHoverColor,
                    tenantInfo.LogoImageUrl,
                    tourDate.StartDate,
                    todaysTours.Data.Count,
                    todaysTours.Data.Count(x => x.BookingIsPaid),
                    todaysTours.Data.Count(x => !x.BookingIsPaid),
                    tenantInfo.Name
                    );
                
                var mailRequest = new MailRequest(
                    to: [property.EmailAddress],
                    subject: $"Today's Tour Manifests for {tour.Name} - {tourDate.StartDate.TimeOfDay}",
                    body: emailHtml,
                    bcc: ["admin@travaloud.com"]!,
                    attachmentData: destinationManifests);
                        
                await _mailService.SendAsync(mailRequest);
            }
        }
    }
}