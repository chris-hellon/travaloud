using System.Data;
using Finbuckle.MultiTenant;
using MediatR;
using Travaloud.Application.BackgroundJobs;
using Travaloud.Application.BackgroundJobs.Commands;
using Travaloud.Application.Catalog.TourDates.Dto;
using Travaloud.Application.Common.Interfaces;
using Travaloud.Application.Common.Persistence;
using Travaloud.Infrastructure.Multitenancy;

namespace Travaloud.Infrastructure.BackgroundJobs;

public class SendDailyTourManifestBatchHandler : IRequestHandler<SendDailyTourManifestBatch>
{
    private readonly IJobService _jobService;
    private readonly IDapperRepository _dapperRepository;
    private readonly IMultiTenantContextAccessor<TravaloudTenantInfo> _multiTenantContextAccessor;
    private readonly ITourManifestService _tourManifestService;
    
    public SendDailyTourManifestBatchHandler(
        IJobService jobService, 
        IDapperRepository dapperRepository, 
        IMultiTenantContextAccessor<TravaloudTenantInfo> multiTenantContextAccessor, 
        ITourManifestService tourManifestService)
    {
        _jobService = jobService;
        _dapperRepository = dapperRepository;
        _multiTenantContextAccessor = multiTenantContextAccessor;
        _tourManifestService = tourManifestService;
    }

    public async Task Handle(SendDailyTourManifestBatch request, CancellationToken cancellationToken)
    {
        var tenant = _multiTenantContextAccessor.MultiTenantContext?.TenantInfo;
        
        var tenantId = tenant?.Id ??
                       throw new Exception("No Tenant found.");

        var currentDate = DateTime.Now + new TimeSpan(0, 0, 0);
        
        var tourDates = await _dapperRepository.QueryAsync<TourDailyManifestDto>("GetTodaysTourDatesForManifest", new
        {
            TenantId = tenantId,
            CurrentDate = currentDate
        }, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
        
        foreach (var tourDate in tourDates)
        { 
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var nowInTimeZone = TimeZoneInfo.ConvertTime(DateTime.Now, timeZone);
                    
            var targetDateTime = tourDate.StartDate.AddHours(-1);
            var targetDateTimeInTimeZone = TimeZoneInfo.ConvertTime(targetDateTime, timeZone);
                    
            var delay = targetDateTimeInTimeZone - nowInTimeZone;
                    
            if (delay < TimeSpan.Zero)
            {
                continue;
            }
                    
            _jobService.Schedule(() => _tourManifestService.ScheduleTourManifest(new SendDailyTourManifest(
                tourDate.TourId,
                tourDate.TourDateId,
                tourDate.DestinationId,
                tenantId,
                tourDate.TourName!,
                tourDate.StartDate,
                tenant.Name,
                tenant.LogoImageUrl,
                tenant.PrimaryHoverColor), cancellationToken), delay);
        }
    }
}
