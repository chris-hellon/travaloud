using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QRCoder;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Infrastructure.Common.Extensions;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.User;

public partial class QRCodes
{
    [Inject] private IToursService ToursService { get; set; }
    [Inject] private IBookingsService BookingsService { get; set; }
    
    private string? _userId;

    private Dictionary<string, string> ToursQRCodes { get; set; } = new();
    
    protected override async Task OnInitializedAsync()
    {
        if ((await AuthState.GetAuthenticationStateAsync()).User is { } userClaims)
        {
            _userId = userClaims.GetUserId();

            if (_userId != null)
            {
                var tours = await ToursService.SearchAsync(new SearchToursRequest()
                {
                    PageNumber = 1,
                    PageSize = 99999
                });

                if (tours != null && tours.Data.Count != 0)
                {
                    foreach (var tour in tours.Data)
                    {
                        using MemoryStream ms = new();
                        QRCodeGenerator qrCodeGenerate = new();
                        var url = await BookingsService.CreateStaffTourBookingQrCodeUrl(new CreateStaffTourBookingQrCodeUrlRequest(_userId, tour.Name));
                        var qrCodeData = qrCodeGenerate.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                        QRCode qrCode = new(qrCodeData);
                        using var qrBitMap = qrCode.GetGraphic(20);
                        qrBitMap.Save(ms, ImageFormat.Png);
                        
                        var base64 = Convert.ToBase64String(ms.ToArray());

                        ToursQRCodes.Add(tour.Name, $"data:image/png;base64,{base64}");
                    }
                }
            }
        }
    }
    
    protected async Task Download(string tourName,string base64Data)
    {   
        base64Data = base64Data.Trim('"');
        await JS.InvokeVoidAsync("triggerFileDownload",  $"{tourName}.png", base64Data);
    }
}