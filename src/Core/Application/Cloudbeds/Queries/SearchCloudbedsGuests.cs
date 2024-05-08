using System.Data;
using Dapper;
using Travaloud.Application.Cloudbeds.Dto;

namespace Travaloud.Application.Cloudbeds.Queries;

public class SearchCloudbedsGuests : IRequest<IEnumerable<GuestDto>>
{
    public List<GuestDto> Guests { get; set; }

    public SearchCloudbedsGuests(List<GuestDto> guests)
    {
        Guests = guests;
    }
}

internal class SearchCloudbedsGuestsHandler : IRequestHandler<SearchCloudbedsGuests, IEnumerable<GuestDto>>
{
    private readonly IDapperRepository _repository;

    public SearchCloudbedsGuestsHandler(IDapperRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<GuestDto>> Handle(SearchCloudbedsGuests request, CancellationToken cancellationToken)
    {
        var dt = new DataTable();
        dt.Columns.Add("GuestId");
        dt.Columns.Add("FirstName");
        dt.Columns.Add("LastName");
        dt.Columns.Add("Gender");
        dt.Columns.Add("PhoneNumber");
        dt.Columns.Add("Email");
        dt.Columns.Add("Nationality");
        dt.Columns.Add("DateOfBirth", Nullable.GetUnderlyingType(typeof(DateTime?)) ?? typeof(DateTime?));
        
        foreach (var guest in request.Guests)
        {
            var dataRow = dt.NewRow();
            dataRow["GuestId"] = guest.GuestId;
            dataRow["FirstName"] = guest.FirstName;
            dataRow["LastName"] = guest.LastName;
            dataRow["Gender"] = guest.Gender;
            dataRow["PhoneNumber"] = guest.Phone;
            dataRow["Email"] = guest.Email;
            dataRow["Nationality"] = guest.Nationality;
            dataRow["DateOfBirth"] = guest.DateOfBirth.HasValue ? guest.DateOfBirth.Value : DBNull.Value;
            dt.Rows.Add(dataRow);
        }
        
        var guests = await _repository.QueryAsync<GuestDto>(
            sql: "SearchCloudbedsGuests",
            param: new
            {
                GuestSearch = dt.AsTableValuedParameter()
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return guests.Select(x =>
        {
            var matchedGuest = request.Guests.FirstOrDefault(g => g.GuestId == x.GuestId);
            if (matchedGuest == null) return x;
            
            x.CustomFields = matchedGuest.CustomFields;
            x.GuestDocumentType = matchedGuest.GuestDocumentType;
            x.GuestDocumentNumber = matchedGuest.GuestDocumentNumber;
            x.GuestDocumentIssueDate = matchedGuest.GuestDocumentIssueDate;
            x.GuestDocumentIssuingCountry = matchedGuest.GuestDocumentIssuingCountry;
            x.GuestDocumentExpirationDate = matchedGuest.GuestDocumentExpirationDate;

            return x;
        });
    }
}