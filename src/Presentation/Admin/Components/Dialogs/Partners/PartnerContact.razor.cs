using Blazored.FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Partners.Commands;
using Travaloud.Application.Catalog.Partners.Dto;
using Travaloud.Infrastructure.Common.Services;

namespace Travaloud.Admin.Components.Dialogs.Partners;

public partial class PartnerContact : ComponentBase
{
    [Parameter] [EditorRequired] public UpdatePartnerContactRequest RequestModel { get; set; } = default!;

    [Parameter] public UpdatePartnerRequest Partner { get; set; } = default!;

    [Parameter] public EntityServerTableContext<PartnerDto, Guid, UpdatePartnerRequest> Context { get; set; } = default!;

    [Parameter] public object? Id { get; set; }

    public EditForm EditForm { get; set; } = default!;

    private bool IsCreate => Id is null;

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
        if (await _fluentValidationValidator!.ValidateAsync())
        {
            if (await ServiceHelper.ExecuteCallGuardedAsync(
                    () =>
                    {
                        if (!IsCreate) return Task.CompletedTask;
                        Partner.PartnerContacts ??= new List<UpdatePartnerContactRequest>();

                        if (Partner.PartnerContacts.Count > 0)
                        {
                            var lastField = Partner.PartnerContacts.Last();
                            if (lastField?.Name != null)
                            {
                                Partner.PartnerContacts.Add(RequestModel.Adapt<UpdatePartnerContactRequest>());
                            }
                        }
                        else
                        {
                            Partner.PartnerContacts.Add(RequestModel.Adapt<UpdatePartnerContactRequest>());
                        }

                        return Task.CompletedTask;
                    },
                    Snackbar,
                    Logger,
                    $"Contact {(IsCreate ? L["Created"] : L["Updated"])}."))
            {
                MudDialog.Close(RequestModel);
            }
        }
        else
        {
            Snackbar.Add("One or more validation errors occurred.");
        }
    }

    public void ShowHelpDialog()
    {
        DialogOptions options = new() {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = false, DisableBackdropClick = true};
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