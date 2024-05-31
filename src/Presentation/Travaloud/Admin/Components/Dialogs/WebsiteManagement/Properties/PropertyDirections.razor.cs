using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Admin.Components.Pages.WebsiteManagement;
using Travaloud.Application.Catalog.Properties.Commands;
using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Infrastructure.Common.Services;

namespace Travaloud.Admin.Components.Dialogs.WebsiteManagement.Properties;

public partial class PropertyDirections : ComponentBase
{
    [Parameter] [EditorRequired] public PropertyDirectionRequest RequestModel { get; set; } = default!;

    [Parameter] public PropertyViewModel Property { get; set; } = default!;

    [Parameter]
    public EntityServerTableContext<PropertyDto, DefaultIdType, PropertyViewModel> Context { get; set; } = default!;

    [Parameter] public object? Id { get; set; }

    public EditForm EditForm { get; set; } = default!;

    private bool IsCreate => Id is null;

    [Parameter] public MudCarousel<PropertyDirectionContentRequest>? DirectionContentsCarousel { get; set; }

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
                        Property.Directions ??= new List<PropertyDirectionRequest>();

                        if (Property.Directions.Any())
                        {
                            var lastPrice = Property.Directions.Last();
                            if (!string.IsNullOrEmpty(lastPrice.Title))
                            {
                                Property.Directions.Add(RequestModel);
                            }
                        }
                        else
                        {
                            Property.Directions.Add(RequestModel);
                        }

                        return Task.CompletedTask;
                    },
                    Snackbar,
                    Logger,
                    $"Property Direction {(IsCreate ? L["Created"] : L["Updated"])}."))
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
}