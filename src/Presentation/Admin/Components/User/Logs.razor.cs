using Mapster;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Auditing;
using Travaloud.Application.Identity;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.User;

public partial class Logs
{
    [Inject] private IPersonalService PersonalService { get; set; } = default!;

    private EntityClientTableContext<RelatedAuditTrail, Guid, object> Context { get; set; } = default!;

    private string? _searchString;
    private MudDateRangePicker _dateRangePicker = default!;
    private DateRange? _dateRange;
    private bool _searchInOldValues;
    private bool _searchInNewValues;
    private List<RelatedAuditTrail> _trails = [];

    // Configure Automapper
    static Logs() =>
        TypeAdapterConfig<AuditDto, RelatedAuditTrail>.NewConfig().Map(
            dest => dest.LocalTime,
            src => DateTime.SpecifyKind(src.DateTime, DateTimeKind.Utc).ToLocalTime());

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState.GetAuthenticationStateAsync();
        var user = authState.User.GetUserId();
        
        Context = new EntityClientTableContext<RelatedAuditTrail, Guid, object>(
            entityNamePlural: L["Logs"],
            searchAction: true.ToString(),
            fields:
            [
                new EntityField<RelatedAuditTrail>(audit => audit.Id, L["Id"]),
                new EntityField<RelatedAuditTrail>(audit => audit.TableName, L["Table Name"]),
                new EntityField<RelatedAuditTrail>(audit => audit.DateTime, L["Date"], Template: DateFieldTemplate),
                new EntityField<RelatedAuditTrail>(audit => audit.Type, L["Type"])
            ],
            loadDataFunc: async () =>
            {
                if (user != null)
                    return _trails = (await PersonalService.GetLogsAsync(Guid.Parse(user)))
                        .Adapt<List<RelatedAuditTrail>>();

                return new List<RelatedAuditTrail>();
            },
            searchFunc: (searchString, trail) =>
                (string.IsNullOrWhiteSpace(searchString) // check Search String
                 || trail.TableName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                 || (_searchInOldValues &&
                     trail.OldValues?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true)
                 || (_searchInNewValues &&
                     trail.NewValues?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true))
                && ((_dateRange?.Start is null && _dateRange?.End is null) // check Date Range
                    || (_dateRange?.Start is not null && _dateRange.End is null && trail.DateTime >= _dateRange.Start)
                    || (_dateRange?.Start is null && _dateRange?.End is not null &&
                        trail.DateTime <= _dateRange.End + new TimeSpan(0, 11, 59, 59, 999))
                    || (trail.DateTime >= _dateRange!.Start &&
                        trail.DateTime <= _dateRange.End + new TimeSpan(0, 11, 59, 59, 999))),
            hasExtraActionsFunc: () => true);
    }

    private void ShowBtnPress(Guid id)
    {
        var trail = _trails.First(f => f.Id == id);
        trail.ShowDetails = !trail.ShowDetails;
        foreach (var otherTrail in _trails.Except(new[] {trail}))
        {
            otherTrail.ShowDetails = false;
        }
    }

    public class RelatedAuditTrail : AuditDto
    {
        public bool ShowDetails { get; set; }
        public DateTime LocalTime { get; set; }
    }
}