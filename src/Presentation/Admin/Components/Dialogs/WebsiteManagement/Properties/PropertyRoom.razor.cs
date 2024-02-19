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

public partial class PropertyRoom : ComponentBase
{
    [Parameter] [EditorRequired] public UpdatePropertyRoomRequest RequestModel { get; set; } = default!;

    [Parameter] public PropertyViewModel Property { get; set; } = default!;

    [Parameter] public EntityServerTableContext<PropertyDto, Guid, PropertyViewModel> Context { get; set; } = default!;

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
        if (await _fluentValidationValidator!.ValidateAsync())
        {
            if (await ServiceHelper.ExecuteCallGuardedAsync(
                    () =>
                    {
                        if (!IsCreate) return Task.CompletedTask;
                        Property.Rooms ??= new List<UpdatePropertyRoomRequest>();

                        if (Property.Rooms.Any())
                        {
                            var lastPrice = Property.Rooms.Last();
                            if (!string.IsNullOrEmpty(lastPrice.Name))
                            {
                                Property.Rooms.Add(RequestModel);
                            }
                        }
                        else
                        {
                            Property.Rooms.Add(RequestModel);
                        }

                        return Task.CompletedTask;
                    },
                    Snackbar,
                    Logger,
                    $"Property Room {(IsCreate ? L["Created"] : L["Updated"])}."))
            {
                MudDialog.Close(RequestModel);
            }
        }
        else
        {
            Snackbar.Add("One or more validation errors occurred.");
        }
    }

    public void ClearImageInBytes()
    {
        RequestModel.ImageInBytes = string.Empty;
        StateHasChanged();
    }

    public void SetDeleteCurrentImageFlag()
    {
        RequestModel.ImageInBytes = string.Empty;
        RequestModel.ImagePath = string.Empty;
        RequestModel.DeleteCurrentImage = true;
        StateHasChanged();
    }

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        var fileUploadDetails = await FileUploadHelper.UploadFile(e, Snackbar);

        if (fileUploadDetails != null)
        {
            RequestModel.ImageExtension = fileUploadDetails.Extension;
            RequestModel.ImageInBytes = fileUploadDetails.FileInBytes;
            RequestModel.DeleteCurrentImage = false;

            StateHasChanged();
        }
    }
}