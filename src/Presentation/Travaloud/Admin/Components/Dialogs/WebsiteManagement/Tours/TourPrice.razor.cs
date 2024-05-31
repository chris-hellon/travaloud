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

public partial class TourPrice : ComponentBase
{
    [Parameter] [EditorRequired] public TourPriceRequest RequestModel { get; set; } = default!;

    [Parameter] public TourViewModel Tour { get; set; } = default!;

    [Parameter] public EntityServerTableContext<TourDto, DefaultIdType, TourViewModel> Context { get; set; } = default!;

    [Parameter] public object? Id { get; set; }

    public EditForm EditForm { get; set; } = default!;

    private bool IsCreate => Id is null;

    private MudTextField<decimal?> DayDuration { get; set; } = default!;

    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;

    private FluentValidationValidator? _fluentValidationValidator;

    private EditContext? EditContext { get; set; }

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
                        Tour.TourPrices ??= new List<TourPriceRequest>();

                        RequestModel.IsCreate = true;
                        RequestModel.Id = DefaultIdType.NewGuid();

                        if (Tour.TourPrices.Any())
                        {
                            var lastPrice = Tour.TourPrices.Last();
                            if (lastPrice?.Price != null)
                            {
                                Tour.TourPrices.Add(RequestModel);
                            }
                        }
                        else
                        {
                            Tour.TourPrices.Add(RequestModel);
                        }

                        return Task.CompletedTask;
                    },
                    Snackbar,
                    Logger,
                    $"Tour Price {(IsCreate ? L["Created"] : L["Updated"])}. Please note this is not final, you must Save the Tour to confirm these Prices."))
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

    private async Task HandleValidation()
    {
        await DayDuration.Validate();
        await DayDuration.FocusAsync();
        await DayDuration.BlurAsync();
    }

    public void ShowHelpDialog()
    {
        DialogOptions options = new()
            {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = false, DisableBackdropClick = true};
        DialogParameters parameters = new()
        {
            {
                nameof(GenericDialog.ContentText),
                "Enter a Price and a Title and Description. A title can just be Per Person, and a Description can be a more detailed label, such as Private Room, VIP, (3D/2N) etc.<br /><br />Next select either a Day, Night or Hour duration. Either of these is a required field, as any Dates created for this price will use these fields to calculate when the date ends.<br /><br />Finally if the price should only run for a certain time in the year, select a Month from and Month to range."
            },
            {nameof(GenericDialog.TitleText), "Guide"}
        };

        DialogService.Show<GenericDialog>(string.Empty, parameters, options);
    }
}