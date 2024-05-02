using Finbuckle.MultiTenant;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Travaloud.Application.BackgroundJobs.Commands;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Properties.Queries;
using Travaloud.Application.Cloudbeds;
using Travaloud.Application.Cloudbeds.Dto;
using Travaloud.Application.Cloudbeds.Queries;
using Travaloud.Application.Common.Extensions;
using Travaloud.Application.Common.Persistence;
using Travaloud.Application.Identity.Users;
using Travaloud.Infrastructure.Identity;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Shared.Authorization;

namespace Travaloud.Infrastructure.BackgroundJobs;

public class ImportCloudbedsGuestsHandler : IRequestHandler<ImportCloudbedsGuests>
{
    private readonly IMultiTenantContextAccessor<TravaloudTenantInfo> _multiTenantContextAccessor;
    private readonly ILogger<ImportCloudbedsGuestsHandler> _logger;
    private readonly IPropertiesService _propertiesService;
    private readonly IUserService _userService;
    private readonly ICloudbedsService _cloudbedsService;
    private readonly IDapperRepository _repository;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public ImportCloudbedsGuestsHandler(
        ICloudbedsService cloudbedsService,
        IPropertiesService propertiesService,
        ILogger<ImportCloudbedsGuestsHandler> logger, 
        IMultiTenantContextAccessor<TravaloudTenantInfo> multiTenantContextAccessor, 
        UserManager<ApplicationUser> userManager, IDapperRepository repository, IUserService userService)
    {
        _cloudbedsService = cloudbedsService;
        _propertiesService = propertiesService;
        _logger = logger;
        _multiTenantContextAccessor = multiTenantContextAccessor;
        _userManager = userManager;
        _repository = repository;
        _userService = userService;
    }

    public async Task Handle(ImportCloudbedsGuests request, CancellationToken cancellationToken)
    {
        var tenantId = _multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Id ??
                       throw new CustomException("No Tenant Found");
        
        var properties = await _propertiesService.SearchAsync(new SearchPropertiesRequest());

        if (properties != null && properties.Data.Count != 0)
        {
            var distinctGuests = new List<GuestDto>();
            
            _logger.LogInformation("Properties: {propertiesCount} ", properties.Data.Count.ToString());
            
            foreach (var property in properties.Data)
            {
                if (property is not {CloudbedsPropertyId: not null, CloudbedsApiKey: not null}) continue;
            
                var propertyResult = await _cloudbedsService.GetGuests(new GetGuestsRequest(property.CloudbedsPropertyId,
                    property.CloudbedsApiKey)
                {
                    ResultsFrom = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd"),
                    ResultsTo = DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd")
                });
                
                if (propertyResult is {Success: true, Data: not null})
                {
                    distinctGuests.AddRange(propertyResult.Data);
                }
            }
            
            var groupedDistinctGuests = distinctGuests
                .GroupBy(x => x.Email) // Group by email address
                .Select(group => new
                {
                    Email = group.Key,
                    Guests = group.ToList().DistinctBy(x => $"{x.FirstName} {x.LastName}") // List of guests with the same email address
                });
            
            var flattenedList = groupedDistinctGuests
                .SelectMany(group =>
                    group.Guests.Select((guest, index) => new GuestDto()
                    {
                        FirstName = guest.FirstName,
                        LastName = guest.LastName,
                        Nationality = guest.Nationality,
                        Gender = guest.Gender,
                        DateOfBirth = guest.DateOfBirth,
                        Phone = guest.Phone,
                        Email = index == 0 ? group.Email : $"+{index}{group.Email}"
                    }))
                .ToList();
            
            var guestsToInsert = await _cloudbedsService.SearchGuests(new SearchCloudbedsGuests(flattenedList));
            
            var guestDtos = guestsToInsert as GuestDto[] ?? guestsToInsert.ToArray();
            if (guestDtos.Any())
            {
                guestDtos = guestDtos.DistinctBy(x => x.Email).ToArray();
                
                // var dt = new DataTable();
                // dt.Columns.Add("FirstName");
                // dt.Columns.Add("LastName");
                // dt.Columns.Add("TenantId");
                // dt.Columns.Add("UserName");
                // dt.Columns.Add("NormalizedUserName");
                // dt.Columns.Add("Email");
                // dt.Columns.Add("NormalizedEmail");
                // dt.Columns.Add("PasswordHash");
                // dt.Columns.Add("PhoneNumber");
                // dt.Columns.Add("DateOfBirth", Nullable.GetUnderlyingType(typeof(DateTime?)) ?? typeof(DateTime?));
                // dt.Columns.Add("Gender");
                // dt.Columns.Add("Nationality");

                var userRequests = new List<CreateUserRequest>();
                
                foreach (var toRegister in guestDtos)
                {
                    // var dataRow = dt.NewRow();
                    //
                    // dataRow["FirstName"] = toRegister.FirstName.TrimStart().ReplaceFunkyFirstnames();
                    // dataRow["LastName"] = toRegister.LastName.ReplaceFunkyFirstnames();
                    // dataRow["TenantId"] = tenantId;
                    // dataRow["UserName"] = toRegister.Email;
                    // dataRow["NormalizedUserName"] = toRegister.Email.Normalize().ToUpper();
                    // dataRow["Email"] = toRegister.Email;
                    // dataRow["NormalizedEmail"] = toRegister.Email.Normalize().ToUpper();
                    // dataRow["PhoneNumber"] = toRegister.Phone;
                    // dataRow["DateOfBirth"] = toRegister.DateOfBirth.HasValue ? toRegister.DateOfBirth.Value : DBNull.Value;
                    // dataRow["Gender"] = toRegister.Gender;
                    // dataRow["Nationality"] = toRegister.Nationality;
                    // dataRow["PasswordHash"] = DBNull.Value;
                    //
                    // dt.Rows.Add(dataRow);

                    userRequests.Add(new CreateUserRequest()
                    {
                        FirstName = toRegister.FirstName.TrimStart().ReplaceFunkyFirstnames(),
                        LastName = toRegister.LastName.ReplaceFunkyFirstnames(),
                        Email = toRegister.Email,
                        PhoneNumber = toRegister.Phone,
                        DateOfBirth = toRegister.DateOfBirth,
                        Gender = toRegister.Gender,
                        Nationality = toRegister.Nationality
                    });
                }

                var message = await _userService.BatchCreateAsync(userRequests, TravaloudRoles.Guest);
                
                // await _repository.ExecuteAsync(
                //     sql: "BatchInsertUsers",
                //     param: new
                //     {
                //         Users = dt.AsTableValuedParameter()
                //     },
                //     commandType: CommandType.StoredProcedure,
                //     cancellationToken: cancellationToken);
                
                _logger.LogInformation(message);
            }
        }
    }
}