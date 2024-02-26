using System.Text.Json;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Admin.Components.Pages.WebsiteManagement.Tours;
using Travaloud.Application.Catalog.Tours.Commands;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Infrastructure.Common.Services;

namespace Travaloud.Admin.Components.Dialogs.WebsiteManagement.Tours;

public partial class TourItinerary : ComponentBase
{
    [Parameter] [EditorRequired] public TourItineraryRequest RequestModel { get; set; } = default!;

    [Parameter] public TourViewModel Tour { get; set; } = default!;

    [Parameter] public EntityServerTableContext<TourDto, Guid, TourViewModel> Context { get; set; } = default!;

    [Parameter] public MudCarousel<TourItinerarySectionRequest>? Carousel { get; set; }

    [Parameter] public object? Id { get; set; }

    public EditForm EditForm { get; set; } = default!;

    private bool IsCreate => Id is null;

    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;

    private EditContext? EditContext { get; set; }

    private FluentValidationValidator? _fluentValidationValidator;

    protected override async Task OnInitializedAsync()
    {
        EditContext = new EditContext(RequestModel);

        await base.OnInitializedAsync();
    }

    private void Cancel() =>
        MudDialog.Cancel();

    private async Task SaveAsync()
    {
        await LoadingService.ToggleLoaderVisibility(true);
        
        if (await _fluentValidationValidator!.ValidateAsync())
        {
            if (await ServiceHelper.ExecuteCallGuardedAsync(
                    () =>
                    {
                        if (!IsCreate) return Task.CompletedTask;
                        Tour.TourItineraries ??= new List<TourItineraryRequest>();

                        RequestModel.IsCreate = true;
                        RequestModel.Id = Guid.NewGuid();

                        if (Tour.TourItineraries.Any())
                        {
                            var lastTourItinerary = Tour.TourItineraries.Last();
                            if (lastTourItinerary.Header != null) Tour.TourItineraries.Add(RequestModel);
                        }
                        else
                        {
                            Tour.TourItineraries.Add(RequestModel);
                        }

                        return Task.CompletedTask;
                    },
                    Snackbar,
                    Logger,
                    $"Tour Itinerary {(IsCreate ? L["Created"] : L["Updated"])}."))
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

    private void AddTourItinerarySection()
    {
        TourItinerarySectionRequest newSection = new()
        {
            IsCreate = true,
            Id = Guid.NewGuid()
        };

        RequestModel.Sections ??= new List<TourItinerarySectionRequest>();

        if (RequestModel.Sections.Any())
        {
            var lastTourItinerary = RequestModel.Sections.Last();
            if (lastTourItinerary?.Title != null)
            {
                RequestModel.Sections.Add(newSection);
                Carousel?.MoveTo(RequestModel.Sections.IndexOf(newSection));
            }
            else
            {
                DialogOptions options = new()
                    {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = false, DisableBackdropClick = true};
                DialogParameters parameters = new()
                {
                    {
                        nameof(GenericDialog.ContentText),
                        "A blank section already exists. Please complete this section before adding a new one."
                    },
                    {nameof(GenericDialog.TitleText), "Alert"}
                };

                DialogService.Show<GenericDialog>(string.Empty, parameters, options);
            }
        }
        else
        {
            RequestModel.Sections.Add(newSection);
            Carousel?.MoveTo(RequestModel.Sections.IndexOf(newSection));
        }

        EditContext = new EditContext(RequestModel);
        StateHasChanged();
    }

    private void DeleteTourItinerarySection(TourItinerarySectionRequest section)
    {
        RequestModel.Sections?.Remove(section);
        StateHasChanged();
    }

    public async Task InvokeImagesDialog(TourItinerarySectionRequest requestModel)
    {
        var initialModel = JsonSerializer.Deserialize<IList<TourItinerarySectionRequest>>(JsonSerializer.Serialize(RequestModel.Sections)) ?? new List<TourItinerarySectionRequest>();
        DialogOptions options = new() {CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true};
        DialogParameters parameters = new()
        {
            {nameof(TourItineraryImages.RequestModel), requestModel},
            {nameof(TourItineraryImages.Tour), Tour},
            {nameof(TourItineraryImages.Id), requestModel.Id}
        };

        var dialog = await DialogService.ShowAsync<TourItineraryImages>(string.Empty, parameters, options);

        var result = await dialog.Result;

        if (result.Canceled)
        {
            RequestModel.Sections = initialModel;
        }

        Context.AddEditModal?.ForceRender();
    }

    public void ShowHelpDialog()
    {
        DialogOptions options = new() {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = false, DisableBackdropClick = true};
        DialogParameters parameters = new()
        {
            {
                nameof(GenericDialog.ContentText),
                "Enter a Title, Header and Description for the itinerary in the left hand side fields.<br /><br />The right hand side contains the sections that will be added to the Itinerary. Complete all the required fields here, and click the Edit Images button to add images to the section.<br /><br />If you'd like to add an additional section, click the Add New Section button in the bottom right corner."
            },
            {nameof(GenericDialog.TitleText), "Guide"}
        };

        DialogService.Show<GenericDialog>(string.Empty, parameters, options);
    }
}