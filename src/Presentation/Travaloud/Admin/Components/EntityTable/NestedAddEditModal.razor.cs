using System.Text.Json;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Travaloud.Infrastructure.Common.Services;

namespace Travaloud.Admin.Components.EntityTable;

public partial class NestedAddEditModal<TRequest> : ComponentBase, INestedAddEditModal<TRequest>
{
    [Parameter]
    [EditorRequired]
    public TRequest RequestModel { get; set; } = default!;

    public TRequest InitialModel { get; set; } = default!;

    [Parameter]
    [EditorRequired]
    public RenderFragment<TRequest> EditFormContent { get; set; } = default!;

    [Parameter]
    [EditorRequired]
    public string EntityName { get; set; } = default!;

    [Parameter]
    public object? Id { get; set; }

    [Parameter]
    [EditorRequired]
    public Func<TRequest, Task> SaveFunc { get; set; } = default!;

    [Parameter]
    [EditorRequired]
    public Func<TRequest, Task>? CancelFunc { get; set; }

    [Parameter]
    [EditorRequired]
    public Func<TRequest, Task> AdditionalAction { get; set; } = default!;

    [Parameter]
    [EditorRequired]
    public string? AdditionalActionText { get; set; }

    public bool IsCreate => Id is null;

    private EditContext? _editContext { get; set; }

    private FluentValidationValidator? _fluentValidationValidator;

    [CascadingParameter]
    private MudDialogInstance MudDialog { get; set; } = default!;

    protected override Task OnInitializedAsync()
    {
        if (RequestModel != null)
        {
            var json = JsonSerializer.Serialize(RequestModel);

            if (json != null)
            {
                InitialModel = JsonSerializer.Deserialize<TRequest>(json) ?? throw new ArgumentNullException(nameof(json));
            }

            _editContext = new EditContext(RequestModel);
        }

        return base.OnInitializedAsync();
    }

    private void Cancel()
    {
        var json = JsonSerializer.Serialize(InitialModel);

        if (json != null)
        {
            RequestModel = JsonSerializer.Deserialize<TRequest>(json) ?? throw new ArgumentNullException(nameof(json));
        }

        //if (CancelFunc != null)
        //{
        //    CancelFunc(RequestModel);
        //}

        MudDialog.Cancel();
    }

    private async Task SaveAsync()
    {
        try
        {
            if (await _fluentValidationValidator!.ValidateAsync())
            {
                if (await ServiceHelper.ExecuteCallGuardedAsync(
                        () => SaveFunc(RequestModel),
                        Snackbar,
                        Logger,
                        $"{EntityName} {(IsCreate ? L["Created"] : L["Updated"])}."))
                {
                    MudDialog.Close();
                }
            }
            else
            {
                Snackbar.Add("One or more validation errors occurred.");
            }
        }
        catch (Exception)
        {
            StateHasChanged();
        }
    }
}