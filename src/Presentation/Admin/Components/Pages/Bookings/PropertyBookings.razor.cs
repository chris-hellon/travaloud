using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Bookings.Queries;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Application.Catalog.Properties.Queries;
using Travaloud.Application.Identity.Users;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.Bookings;

public partial class PropertyBookings
{
    [Inject] protected IBookingsService BookingsService { get; set; } = default!;

    [Inject] protected IPropertiesService PropertiesService { get; set; } = default!;

    [Inject] protected IUserService UserService { get; set; } = default!;

    [Parameter] public string Category { get; set; } = default!;

    private EntityServerTableContext<BookingDto, Guid, PropertyBookingViewModel> Context { get; set; } = default!;

    private EditContext? EditContext { get; set; }

    private EntityTable<BookingDto, Guid, PropertyBookingViewModel> _table = default!;

    private MudDateRangePicker _dateRangePicker = default!;

    private ICollection<PropertyDto> Properties { get; set; } = default!;

    protected override void OnInitialized()
    {
        Context = new EntityServerTableContext<BookingDto, Guid, PropertyBookingViewModel>(
            entityName: L["Booking"],
            entityNamePlural: L["Bookings"],
            entityResource: TravaloudResource.Bookings,
            fields:
            [
                new EntityField<BookingDto>(booking => booking.Description, L["Description"], "Description"),
                new EntityField<BookingDto>(booking => booking.IsPaid, L["Is Paid"], "IsPaid"),
                new EntityField<BookingDto>(booking => booking.BookingDate, L["Booking Date"], "BookingDate")
            ],
            enableAdvancedSearch: false,
            idFunc: booking => booking.Id,
            searchFunc: async (filter) =>
            {
                var properties = await PropertiesService.SearchAsync(new SearchPropertiesRequest
                    {PageNumber = 1, PageSize = 99999});
                
                Properties = properties?.Data!;

                var adaptedFilter = filter.Adapt<SearchBookingsRequest>();
                adaptedFilter.IsTours = false;
                adaptedFilter.PropertyId = SearchPropertyId;
                adaptedFilter.BookingStartDate = SearchDateRange?.Start;
                adaptedFilter.BookingEndDate = SearchDateRange?.End;

                if (adaptedFilter.BookingEndDate.HasValue)
                {
                    adaptedFilter.BookingEndDate =
                        adaptedFilter.BookingEndDate.Value + new TimeSpan(0, 11, 59, 59, 999);
                }

                var request = await BookingsService.SearchAsync(adaptedFilter);

                return request.Adapt<PaginationResponse<BookingDto>>();
            },
            createAction: string.Empty,
            updateAction: string.Empty,
            viewAction: string.Empty,
            deleteAction: string.Empty
        );
    }

    private string? _searchDescription;

    private string SearchDescription
    {
        get => _searchDescription ?? string.Empty;
        set
        {
            _searchDescription = value;
            _ = _table.ReloadDataAsync();
        }
    }

    private DateTime? _searchBookingDate;

    private DateTime? SearchBookingDate
    {
        get => _searchBookingDate;
        set
        {
            _searchBookingDate = value;
            _ = _table.ReloadDataAsync();
        }
    }

    private Guid? _searchPropertyId;

    private Guid? SearchPropertyId
    {
        get => _searchPropertyId;
        set
        {
            _searchPropertyId = value;
            _ = _table.ReloadDataAsync();
        }
    }

    private DateRange? _searchDateRange;

    private DateRange? SearchDateRange
    {
        get => _searchDateRange;
        set
        {
            _searchDateRange = value;
            _ = _table.ReloadDataAsync();
        }
    }

    private void ShowBookingItems(BookingDto request)
    {
        request.ShowDetails = !request.ShowDetails;
        if (Context.EntityList == null) return;
        foreach (var otherTrail in Context.EntityList.Where(x => x.Id != request.Id))
        {
            otherTrail.ShowDetails = false;
        }
    }

    private static void ShowBookingItemRooms(BookingItemDetailsDto request, BookingDto bookingItem)
    {
        request.ShowDetails = !request.ShowDetails;
        if (bookingItem.Items == null) return;
        foreach (var otherTrail in bookingItem.Items.Where(x => x.Id != request.Id))
        {
            otherTrail.ShowDetails = false;
        }
    }
}

public class PropertyBookingViewModel : UpdateBookingRequest
{
    public ICollection<UserDetailsDto>? Guests { get; set; }
    public ICollection<PropertyDto>? Properties { get; set; }
}