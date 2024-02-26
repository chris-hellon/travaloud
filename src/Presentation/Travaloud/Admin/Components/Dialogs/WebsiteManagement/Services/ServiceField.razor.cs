using System.Text.Json;
using Blazored.FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Services.Commands;
using Travaloud.Application.Catalog.Services.Dto;
using Travaloud.Infrastructure.Common.Services;

namespace Travaloud.Admin.Components.Dialogs.WebsiteManagement.Services;

public partial class ServiceField : ComponentBase
{
    [Inject] public IJSRuntime js { get; set; } = default!;

    [Parameter] [EditorRequired] public UpdateServiceFieldRequest RequestModel { get; set; } = default!;

    [Parameter] public UpdateServiceRequest Service { get; set; } = default!;

    [Parameter]
    public EntityServerTableContext<ServiceDto, Guid, UpdateServiceRequest> Context { get; set; } = default!;

    [Parameter] public object? Id { get; set; }

    public EditForm EditForm { get; set; } = default!;

    private bool IsCreate => Id is null;

    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;

    private FluentValidationValidator? _fluentValidationValidator;

    private EditContext? EditContext { get; set; }

    private MudTable<ServiceFieldSelectOption> ServiceFieldOptionsTable { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if (!string.IsNullOrEmpty(RequestModel.SelectOptions))
        {
            RequestModel.SelectOptionsParsed =
                JsonSerializer.Deserialize<List<ServiceFieldSelectOption>>(RequestModel.SelectOptions);
        }

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
                        string? listName = null;
                        switch (RequestModel.FieldType)
                        {
                            case "select":
                                listName = "SelectOptionsParsed";
                                break;
                            case "radiobuttonlist":
                                listName = "RadioButtonListParsed";
                                break;
                            case "checkboxlist":
                                listName = "CheckboxListParsed";
                                break;
                            case "nationality":
                                RequestModel.SelectOptionsParsed = ListHelpers.GetNationalities()
                                    .Select(x => new ServiceFieldSelectOption() {Key = x.Value, Value = x.Value})
                                    .ToList();
                                break;
                        }

                        if (RequestModel.SelectOptionsParsed != null)
                        {
                            RequestModel.SelectOptionsParsed = RequestModel.SelectOptionsParsed.Select(x =>
                            {
                                if (listName != null)
                                {
                                    x.ListName = listName;
                                }

                                if (x.Key != null) x.Value = x.Key;
                                return x;
                            }).ToList();

                            RequestModel.SelectOptions = JsonSerializer.Serialize(RequestModel.SelectOptionsParsed);
                        }

                        if (!IsCreate) return Task.CompletedTask;
                        Service.ServiceFields ??= new List<UpdateServiceFieldRequest>();

                        if (Service.ServiceFields.Count > 0)
                        {
                            var lastField = Service.ServiceFields.Last();
                            if (lastField?.Label != null)
                            {
                                Service.ServiceFields.Add(RequestModel.Adapt<UpdateServiceFieldRequest>());
                            }
                        }
                        else
                        {
                            Service.ServiceFields.Add(RequestModel.Adapt<UpdateServiceFieldRequest>());
                        }

                        return Task.CompletedTask;
                    },
                    Snackbar,
                    Logger,
                    $"Field {(IsCreate ? L["Created"] : L["Updated"])}."))
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

    public void RemoveFieldOption(ServiceFieldSelectOption option)
    {
        RequestModel.SelectOptionsParsed?.Remove(option);
        StateHasChanged();
    }

    private void AddNewFieldOption()
    {
        RequestModel.SelectOptionsParsed ??= [];

        if (RequestModel.SelectOptionsParsed.Count > 0)
        {
            var lastField = RequestModel.SelectOptionsParsed.Last();
            if (lastField.Key is {Length: > 0})
            {
                RequestModel.SelectOptionsParsed.Add(new ServiceFieldSelectOption());
            }
        }
        else
        {
            RequestModel.SelectOptionsParsed.Add(new ServiceFieldSelectOption());
        }

        StateHasChanged();
    }
}