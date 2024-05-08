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
    
    public ImportCloudbedsGuestsHandler(
        ICloudbedsService cloudbedsService,
        IPropertiesService propertiesService,
        ILogger<ImportCloudbedsGuestsHandler> logger, 
        IMultiTenantContextAccessor<TravaloudTenantInfo> multiTenantContextAccessor, 
        IUserService userService)
    {
        _cloudbedsService = cloudbedsService;
        _propertiesService = propertiesService;
        _logger = logger;
        _multiTenantContextAccessor = multiTenantContextAccessor;
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

                var statuses = new string[] { "in_house", "not_checked_in" };

                foreach (var status in statuses)
                {
                    var propertyResult = await _cloudbedsService.GetGuests(new GetGuestsRequest(property.CloudbedsPropertyId, property.CloudbedsApiKey)
                    {
                        // ResultsFrom = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd"),
                        // ResultsTo = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd"),
                        Status = status
                    });
                
                    if (propertyResult is {Success: true, Data: not null})
                    {
                        distinctGuests.AddRange(propertyResult.Data);
                    }
                }
            }
            
            var groupedDistinctGuests = distinctGuests
                .Where(x => !string.IsNullOrEmpty(x.Email))
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
                        Email = index == 0 ? group.Email : $"+{index}{group.Email}",
                        GuestId = guest.GuestId,
                        CustomFields = guest.CustomFields,
                        GuestDocumentType = guest.GuestDocumentType,
                        GuestDocumentNumber = guest.GuestDocumentNumber,
                        GuestDocumentIssueDate = guest.GuestDocumentIssueDate,
                        GuestDocumentIssuingCountry = guest.GuestDocumentIssuingCountry,
                        GuestDocumentExpirationDate = guest.GuestDocumentExpirationDate
                    }))
                .ToList();

            var usersWithoutEmail = distinctGuests.Where(x => string.IsNullOrEmpty(x.Email));
            flattenedList = flattenedList.Union(usersWithoutEmail).ToList();

            var guestsToInsert = await _cloudbedsService.SearchGuests(new SearchCloudbedsGuests(flattenedList));
            
            var guestDtos = guestsToInsert as GuestDto[] ?? guestsToInsert.ToArray();
            if (guestDtos.Any())
            {
                var createUserRequests = new List<CreateUserRequest>();
                var updateUserRequests = new List<UpdateUserRequest>();
                
                var fieldsToGet = new string[] {"Passport Number", "pp", "Passport number", "passport number"};
                
                foreach (var toRegister in guestDtos)
                {
                    var hasPassportDocumentType = false;
                    if (!string.IsNullOrEmpty(toRegister.GuestDocumentType) && toRegister.GuestDocumentType == "Passport" && !string.IsNullOrEmpty(toRegister.GuestDocumentNumber))
                    {
                        toRegister.PassportNumber = toRegister.GuestDocumentNumber;
                        hasPassportDocumentType = true;
                    }
                    else
                    {
                        var passportCustomField = toRegister.CustomFields?.FirstOrDefault(x => fieldsToGet.Contains(x.CustomFieldName));
                    
                        if (passportCustomField != null)
                            toRegister.PassportNumber = passportCustomField.CustomFieldValue;
                    }

                    DateTime? guestDocumentExpirationDate = null;
                    
                    if (DateTime.TryParse(toRegister.GuestDocumentExpirationDate, out var guestDocumentExpirationDateResult))
                    {
                        guestDocumentExpirationDate = guestDocumentExpirationDateResult;
                    }
                    
                    DateTime? guestDocumentIssueDate = null;
                    
                    if (DateTime.TryParse(toRegister.GuestDocumentIssueDate, out var guestDocumentIssueDateResult))
                    {
                        guestDocumentIssueDate = guestDocumentIssueDateResult;
                    }
                    
                    if (string.IsNullOrEmpty(toRegister.Id))
                    {
                        createUserRequests.Add(new CreateUserRequest()
                        {
                            FirstName = toRegister.FirstName.TrimStart().ReplaceFunkyFirstnames(),
                            LastName = toRegister.LastName.ReplaceFunkyFirstnames(),
                            Email = toRegister.Email,
                            PhoneNumber = toRegister.Phone,
                            DateOfBirth = toRegister.DateOfBirth,
                            Gender = toRegister.Gender.GenderMatch(),
                            Nationality = toRegister.Nationality.Length == 2 ? toRegister.Nationality.TwoLetterCodeToCountry() : toRegister.Nationality,
                            CloudbedsGuestId = toRegister.GuestId,
                            PassportNumber = toRegister.PassportNumber,
                            PassportExpiryDate = hasPassportDocumentType ? guestDocumentExpirationDate : null,
                            PassportIssuingCountry = hasPassportDocumentType && toRegister.GuestDocumentIssuingCountry?.Length == 2 ? toRegister.GuestDocumentIssuingCountry.TwoLetterCodeToCountry() : toRegister.GuestDocumentIssuingCountry,
                            PassportIssueDate = hasPassportDocumentType ? guestDocumentIssueDate : null
                        });
                    }
                    else
                    {
                        updateUserRequests.Add(new UpdateUserRequest()
                        {
                            Id = toRegister.Id,
                            FirstName = toRegister.FirstName.TrimStart().ReplaceFunkyFirstnames(),
                            LastName = toRegister.LastName.ReplaceFunkyFirstnames(),
                            Email = toRegister.Email,
                            PhoneNumber = toRegister.Phone,
                            DateOfBirth = toRegister.DateOfBirth,
                            Gender = toRegister.Gender.GenderMatch(),
                            Nationality = toRegister.Nationality.Length == 2 ? toRegister.Nationality.TwoLetterCodeToCountry() : toRegister.Nationality,
                            CloudbedsGuestId = toRegister.GuestId,
                            PassportNumber = toRegister.PassportNumber,
                            PassportExpiryDate = hasPassportDocumentType ? guestDocumentExpirationDate : null,
                            PassportIssuingCountry = hasPassportDocumentType && toRegister.GuestDocumentIssuingCountry?.Length == 2 ? toRegister.GuestDocumentIssuingCountry.TwoLetterCodeToCountry() : toRegister.GuestDocumentIssuingCountry,
                            PassportIssueDate = hasPassportDocumentType ? guestDocumentIssueDate : null
                        });
                    }
                }

                if (createUserRequests.Count != 0)
                {
                    var insertMessage = await _userService.BatchCreateAsync(createUserRequests, TravaloudRoles.Guest);
                
                    _logger.LogInformation(insertMessage);
                }
   
                if (updateUserRequests.Count != 0)
                {
                    updateUserRequests = updateUserRequests.DistinctBy(x => x.Id).ToList(); 
                    
                    var updateMessage = await _userService.BatchUpdateAsync(updateUserRequests);
                    _logger.LogInformation(updateMessage);
                }
            }
        }
    }
}