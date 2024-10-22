using MediatR;
using Travaloud.Application.BackgroundJobs.Commands;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Properties.Queries;
using Travaloud.Application.Common.Interfaces;
using Travaloud.Application.Common.Mailing;
using Travaloud.Application.Dashboard;

namespace Travaloud.Infrastructure.BackgroundJobs;

public class SendDailyTourManifestHandler : IRequestHandler<SendDailyTourManifest>
{
    private readonly IDashboardService _dashboardService;
    private readonly IExcelWriter _excelWriter;
    private readonly IPropertiesService _propertiesService;
    private readonly IMailService _mailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly IJobService _jobService;

    public SendDailyTourManifestHandler(
        IDashboardService dashboardService,
        IExcelWriter excelWriter,
        IPropertiesService propertiesService,
        IMailService mailService, 
        IEmailTemplateService emailTemplateService, IJobService jobService)
    {
        _dashboardService = dashboardService;
        _excelWriter = excelWriter;
        _propertiesService = propertiesService;
        _mailService = mailService;
        _emailTemplateService = emailTemplateService;
        _jobService = jobService;
    }

    public async Task Handle(SendDailyTourManifest request, CancellationToken cancellationToken)
    {
        var adaptedFilter = new GetBookingItemsByDateRequest
        {
            TourDateId = request.TourDateId,
            TourId = request.TourId,
            HideRefunded = true,
            TenantId = request.TenantId
        };

        var destinationManifests = new Dictionary<string, byte[]>();
        
        var todaysTours = await _dashboardService.GetTourBookingItemsByDateAsync(adaptedFilter);

        if (todaysTours.Data.Count != 0)
        {
            var fileStream = _excelWriter.WriteToStream(todaysTours.Data);

            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream, cancellationToken);
        
            destinationManifests.Add($"{request.TourName} {request.TourStartDate:hh:mm tt}.xlsx", memoryStream.ToArray());
        
            var properties = await _propertiesService.GetPropertiesByDestination(new GetPropertiesByDestinationRequest(request.TenantId, request.DestinationId));

            foreach (var property in properties)
            {
                if (string.IsNullOrEmpty(property.EmailAddress)) continue;

                var emailHtml = _emailTemplateService.GenerateTourManifestEmail(
                    request.TourName,
                    request.TenantBackgroundColor,
                    request.TenantLogoImageUrl,
                    request.TourStartDate,
                    todaysTours.Data.Count,
                    todaysTours.Data.Count(x => x.BookingIsPaid),
                    todaysTours.Data.Count(x => !x.BookingIsPaid),
                    request.TenantName
                    );
                
                var mailRequest = new MailRequest(
                    to: [property.EmailAddress],
                    subject: $"Today's Tour Manifests for {request.TourName} - {request.TourStartDate.TimeOfDay}",
                    body: emailHtml,
                    attachmentData: destinationManifests);

                _jobService.Enqueue(() => _mailService.SendAsync(mailRequest));
            }
        }
    }
}