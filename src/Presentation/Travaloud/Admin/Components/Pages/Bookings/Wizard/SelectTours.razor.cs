using System.Globalization;
using Mapster;
using Microsoft.AspNetCore.Components;
using MudExtensions;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Common.Extensions;
using Travaloud.Application.Identity.Users;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Shared.Authorization;
using Travaloud.Shared.Multitenancy;

namespace Travaloud.Admin.Components.Pages.Bookings.Wizard;

public partial class SelectTours : ComponentBase
{
    [Inject] protected IUserService UserService { get; set; } = default!;

    [Parameter] public required TourBookingViewModel RequestModel { get; set; }
    
    [CascadingParameter] private MudStepper? _stepper { get; set; }
}