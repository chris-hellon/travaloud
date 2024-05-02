using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using MudExtensions;
using MudExtensions.Enums;
using Travaloud.Infrastructure.Common.Services;

namespace Travaloud.Admin.Components.EntityTable;

public partial class AddEditModal<TRequest> : IAddEditModal<TRequest>
{
    [Parameter] [EditorRequired] public RenderFragment<TRequest> EditFormContent { get; set; } = default!;

    [Parameter] public WizardStep<TRequest>? WizardStep1 { get; set; }
    
    [Parameter] public RenderFragment<TRequest>? WizardStep1Content { get; set; }

    [Parameter] public RenderFragment<TRequest>? WizardStep2Content { get; set; }

    [Parameter] public RenderFragment<TRequest>? WizardStep3Content { get; set; }

    [Parameter] public RenderFragment<TRequest>? WizardStep4Content { get; set; }

    [Parameter] public RenderFragment<TRequest>? WizardStep5Content { get; set; }

    [Parameter] public RenderFragment<TRequest>? WizardStep6Content { get; set; }

    [Parameter] public RenderFragment<TRequest>? WizardStep7Content { get; set; }

    [Parameter] public RenderFragment<TRequest>? WizardStep8Content { get; set; }

    [Parameter] [EditorRequired] public TRequest RequestModel { get; set; } = default!;
    
    [Parameter] [EditorRequired] public Func<TRequest, Task> SaveFunc { get; set; } = default!;
    
    [Parameter] public Func<Task>? OnInitializedFunc { get; set; }
    
    [Parameter] [EditorRequired] public string EntityName { get; set; } = default!;
    
    [Parameter] public object? Id { get; set; }

    [Parameter] public bool IsWizard { get; set; } 
    
    [Parameter] public bool IsFullScreenModal { get; set; }

    [Parameter] public Dictionary<string, bool>? WizardSteps { get; set; }

    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;

    [Parameter] public bool CanSaveEntity { get; set; }
    
    public bool IsCreate => Id is null;

    public bool IsView => SaveFunc is null;

    public void ForceRender() => StateHasChanged();

    public int WizardIndex = 0;
    
    private EditContext? EditContext { get; set; }

    private bool NextDisabled { get; set; } = true;

    private bool LoaderVisible { get; set; } = false;

    private readonly List<MudForm> _wizardSteps = [];

    private MudStepper? _stepper;

    private const bool AddResultStep = true;

    private FluentValidationValidator? _fluentValidationValidator;
    
    protected override Task OnInitializedAsync()
    {
        OnInitializedFunc?.Invoke();

        if (RequestModel != null)
            EditContext = new EditContext(RequestModel);

        if (WizardSteps == null) return base.OnInitializedAsync();
        
        foreach (var wizardStep in WizardSteps)
        {
            _wizardSteps.Add(new MudForm());
        }

        return base.OnInitializedAsync();
    }

    private async Task SaveAsync()
    {
        LoaderVisible = true;
        StateHasChanged();

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

                    LoaderVisible = false;
                    StateHasChanged();
                }
                else
                {
                    LoaderVisible = false;
                    StateHasChanged();
                }
            }
            else
            {
                Snackbar.Add("One or more validation errors occurred.", Severity.Error);
                LoaderVisible = false;
            }
        }
        catch (Exception ex)
        {
            LoaderVisible = false;
            Snackbar.Add(ex.Message, Severity.Error);
            
            StateHasChanged();
        }
    }

    private void Cancel() =>
        MudDialog.Cancel();

    private bool CheckChange()
    {
        var activeIndex = _stepper?.GetActiveIndex();

        if (!activeIndex.HasValue) return false;
        if (_wizardSteps?.ElementAtOrDefault(activeIndex.Value) == null) return false;
        var form = _wizardSteps[activeIndex.Value];
        form.Validate();

        return !form.IsValid;
    }

    private bool CheckChange(StepChangeDirection direction)
    {
        var activeIndex = _stepper?.GetActiveIndex();

        if (!activeIndex.HasValue) return false;
        if (_wizardSteps?.ElementAtOrDefault(activeIndex.Value) == null) return false;
        var form = _wizardSteps[activeIndex.Value];

        // Always allow stepping backwards, even if forms are invalid
        if (direction == StepChangeDirection.Backward)
        {
            return false;
        }

        if (activeIndex.Value == 0)
        {
            form.Validate();
            return !form.IsValid;
        }
        else
        {
            return false;
        }
    }

    private KeyValuePair<string, bool>? GetWizardStep(int index)
    {
        return WizardSteps?.ElementAtOrDefault(index);
    }

    private bool ShowWizardStep(int index)
    {
        var wizardStep = GetWizardStep(index);
        return wizardStep is {Value: true};
    }

    private string WizardStepTitle(int index)
    {
        var wizardStep = GetWizardStep(index);
        return wizardStep != null ? wizardStep.Value.Key : string.Empty;
    }

    private void StatusChanged(StepStatus status)
    {
        if (status != StepStatus.Skipped)
        {
            Snackbar.Add($"Step {Infrastructure.Common.Extensions.EnumExtensions.ToDescriptionString(status)}.", Severity.Success);
        }
    }
}