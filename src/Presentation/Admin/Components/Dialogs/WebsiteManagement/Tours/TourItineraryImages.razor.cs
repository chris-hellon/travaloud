using System.Text.Json;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Travaloud.Admin.Components.Pages.WebsiteManagement.Tours;
using Travaloud.Application.Catalog.Tours.Commands;
using Travaloud.Infrastructure.Common.Services;

namespace Travaloud.Admin.Components.Dialogs.WebsiteManagement.Tours;

public partial class TourItineraryImages : ComponentBase
{
    [Parameter] [EditorRequired] public TourItinerarySectionRequest RequestModel { get; set; } = default!;

    [Parameter] public TourViewModel Tour { get; set; } = default!;

    [Parameter] public MudCarousel<TourItinerarySectionImageRequest>? Carousel { get; set; }

    [Parameter] public object? Id { get; set; }

    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;

    public bool IsCreate => Id is null;

    public TourItinerarySectionRequest InitialModel { get; set; } = default!;

    private FluentValidationValidator? _fluentValidationValidator;

    private EditContext? EditContext { get; set; }

    protected override Task OnInitializedAsync()
    {
        var json = JsonSerializer.Serialize(RequestModel);

        InitialModel = JsonSerializer.Deserialize<TourItinerarySectionRequest>(json) ??
                       throw new ArgumentNullException(nameof(json));

        EditContext = new EditContext(RequestModel);
        
        return base.OnInitializedAsync();
    }

    private void Cancel() =>
        MudDialog.Cancel();

    private async Task SaveAsync()
    {
        await LoadingService.ToggleLoaderVisibility(true);
        
        if (await _fluentValidationValidator!.ValidateAsync())
        {
            if (await ServiceHelper.ExecuteCallGuardedAsync(
                    () => Task.CompletedTask,
                    Snackbar,
                    Logger,
                    $"Tour Itinerary Images {(IsCreate ? L["Created"] : L["Updated"])}."))
            {
                MudDialog.Close(RequestModel);
            }
        }
        else
        {
            Snackbar.Add("One or more validation errors occurred.");
        }
        
        await LoadingService.ToggleLoaderVisibility(false);
    }

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        var fileUploadDetails = await FileUploadHelper.UploadFile(e, Snackbar);

        if (fileUploadDetails != null)
        {
            var newImageRequest = new TourItinerarySectionImageRequest()
            {
                ImageExtension = fileUploadDetails.Extension,
                ImageInBytes = fileUploadDetails.FileInBytes,
                ImagePath = fileUploadDetails.FileInBytes,
                ThumbnailImagePath = fileUploadDetails.FileInBytes
            };
            
            RequestModel.Images ??= new List<TourItinerarySectionImageRequest>();

            RequestModel.Images.Insert(0, newImageRequest);

            StateHasChanged();
        }
    }
}