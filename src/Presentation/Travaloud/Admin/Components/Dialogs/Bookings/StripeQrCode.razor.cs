using Microsoft.AspNetCore.Components;

namespace Travaloud.Admin.Components.Dialogs.Bookings;

public partial class StripeQrCode
{
    [Parameter] public string? QrCodeImageUrl { get; set; }
}