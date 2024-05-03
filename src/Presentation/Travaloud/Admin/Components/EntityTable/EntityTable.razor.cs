using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Mono.TextTemplating;
using MudBlazor;
using Travaloud.Admin.Components.Dialogs;
using Travaloud.Admin.Components.Layout;
using Travaloud.Application.Common.Models;
using Travaloud.Infrastructure.Auth;
using Travaloud.Infrastructure.Common.Services;

namespace Travaloud.Admin.Components.EntityTable;

public partial class EntityTable<TEntity, TId, TRequest>
    where TRequest : new()
{
    [Parameter] [EditorRequired] public EntityTableContext<TEntity, TId, TRequest> Context { get; set; } = default!;

    [Parameter] public bool Loading { get; set; }

    [Parameter] public string? SearchString { get; set; }
    [Parameter] public EventCallback<string> SearchStringChanged { get; set; }

    [Parameter] public RenderFragment? AdvancedSearchContent { get; set; }

    [Parameter] public RenderFragment<TEntity>? ActionsContent { get; set; }
    [Parameter] public RenderFragment<TEntity>? ExtraActions { get; set; }
    [Parameter] public RenderFragment<TEntity>? ChildRowContent { get; set; }

    [Parameter] public RenderFragment<TRequest>? EditFormContent { get; set; }

    [Parameter] public WizardStep<TRequest>? WizardStep1 { get; set; }
    
    [Parameter] public RenderFragment<TRequest>? WizardStep1Content { get; set; }

    [Parameter] public RenderFragment<TRequest>? WizardStep2Content { get; set; }

    [Parameter] public RenderFragment<TRequest>? WizardStep3Content { get; set; }

    [Parameter] public RenderFragment<TRequest>? WizardStep4Content { get; set; }

    [Parameter] public RenderFragment<TRequest>? WizardStep5Content { get; set; }

    [Parameter] public RenderFragment<TRequest>? WizardStep6Content { get; set; }

    [Parameter] public RenderFragment<TRequest>? WizardStep7Content { get; set; }

    [Parameter] public RenderFragment<TRequest>? WizardStep8Content { get; set; }

    [Parameter] public bool IsWizard { get; set; }

    [Parameter] public Dictionary<string, bool>? WizardSteps { get; set; }

    [Parameter] public MaxWidth ModalWidth { get; set; } = MaxWidth.ExtraLarge;
    
    [Parameter] public bool FullScreenModal { get; set; }

    [Parameter] public int Elevation { get; set; } = 25;

    [Parameter] public bool ShowActionsMenu { get; set; } = true;
    
    [Inject] protected IAuthorizationService AuthService { get; set; } = default!;

    private bool _canSearch;
    private bool _canCreate;
    private bool _canUpdate;
    private bool _canDelete;
    private bool _canExport;
    private bool _canView;
    private bool _editNavigateTo;
    private bool _createNavigateTo;

    private bool _advancedSearchExpanded;

    private MudTable<TEntity> _table = default!;
    public IEnumerable<TEntity>? EntityList;
    private int _totalItems;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState.GetAuthenticationStateAsync();
        _canSearch = await CanDoActionAsync(Context.SearchAction, state);
        _canCreate = await CanDoActionAsync(Context.CreateAction, state);
        _canUpdate = await CanDoActionAsync(Context.UpdateAction, state);
        _canDelete = await CanDoActionAsync(Context.DeleteAction, state);
        _canExport = await CanDoActionAsync(Context.ExportAction, state);
        _canView = await CanDoActionAsync(Context.ViewAction, state);
        _editNavigateTo = Context.EditNavigateTo != null;
        _createNavigateTo = Context.CreateNavigateTo != null;
        
        await LocalLoadDataAsync();
    }

    public Task ReloadDataAsync() =>
        Context.IsClientContext
            ? LocalLoadDataAsync()
            : ServerLoadDataAsync();

    private async Task<bool> CanDoActionAsync(string? action, AuthenticationState state) =>
        !string.IsNullOrWhiteSpace(action) &&
        ((bool.TryParse(action, out var isTrue) && isTrue) || // check if action equals "True", then it's allowed
         (Context.EntityResource is { } resource &&
          await AuthService.HasPermissionAsync(state.User, action, resource)));

    private bool HasExtraActions => Context.HasExtraActionsFunc is not null;
    private bool HasActions => _canUpdate || _canDelete || _canView;

    private bool CanUpdateEntity(TEntity entity) =>
        _canUpdate && (Context.CanUpdateEntityFunc is null || Context.CanUpdateEntityFunc(entity));

    private bool CanViewEntity(TEntity entity) =>
        _canView && (Context.CanViewEntityFunc is null || Context.CanViewEntityFunc(entity));

    private bool CanDeleteEntity(TEntity entity) =>
        _canDelete && (Context.CanDeleteEntityFunc is null || Context.CanDeleteEntityFunc(entity));

    private bool LoadEditPage(TEntity entity) =>
        _editNavigateTo && (Context.CanUpdateEntityFunc is null || Context.CanUpdateEntityFunc(entity));

    private bool LoadCreatePage() => _createNavigateTo;

    // Client side paging/filtering
    private bool LocalSearch(TEntity entity) =>
        Context.ClientContext?.SearchFunc is { } searchFunc
            ? searchFunc(SearchString, entity)
            : string.IsNullOrWhiteSpace(SearchString);

    private async Task LocalLoadDataAsync()
    {
        if (Loading || Context.ClientContext is null)
        {
            return;
        }

        Loading = true;

        if (await ServiceHelper.ExecuteCallGuardedAsync(
                () => Context.ClientContext.LoadDataFunc(), Snackbar,
                Logger)
            is List<TEntity> result)
        {
            EntityList = result;
        }

        Loading = false;
    }

    // Server Side paging/filtering

    private async Task OnSearchStringChanged(string? text = null)
    {
        await SearchStringChanged.InvokeAsync(SearchString);

        await ServerLoadDataAsync();
    }

    private async Task ServerLoadDataAsync()
    {
        if (Context.IsServerContext)
        {
            await _table.ReloadServerData();
        }
    }

    private Func<TableState, Task<TableData<TEntity>>>? ServerReloadFunc =>
        Context.IsServerContext ? ServerReload : null;

    private async Task<TableData<TEntity>> ServerReload(TableState state)
    {
        if (!Loading && Context.ServerContext is not null)
        {
            Loading = true;

            var filter = GetPaginationFilter(state);

            if (await ServiceHelper.ExecuteCallGuardedAsync(
                    () => Context.ServerContext.SearchFunc(filter),Snackbar,
                    Logger)
                is { } result)
            {
                _totalItems = result.TotalCount;
                EntityList = result.Data;
                Context.EntityList = EntityList;
            }

            Loading = false;
        }

        return new TableData<TEntity> {TotalItems = _totalItems, Items = EntityList};
    }

    private async Task ExportAsync()
    {
        if (!Loading && Context.ServerContext is not null)
        {
            if (Context.ServerContext.ExportFunc is not null)
            {
                Loading = true;

                var filter = GetBaseFilter();

                if (await ServiceHelper.ExecuteCallGuardedAsync(
                        () => Context.ServerContext.ExportFunc(filter),Snackbar,
                        Logger)
                    is { } result)
                {
                    using var streamRef = new DotNetStreamReference(result.Stream);
                    await JS.InvokeVoidAsync("downloadFileFromStream", $"{Context.EntityNamePlural}.xlsx", streamRef);
                }

                Loading = false;
            }
        }
    }

    private PaginationFilter GetPaginationFilter(TableState state)
    {
        string[]? orderings = null;
        if (!string.IsNullOrEmpty(state.SortLabel))
        {
            orderings = state.SortDirection == SortDirection.None
                ? new[] {$"{state.SortLabel}"}
                : new[] {$"{state.SortLabel} {state.SortDirection}"};
        }

        var filter = new PaginationFilter
        {
            PageSize = state.PageSize,
            PageNumber = state.Page + 1,
            Keyword = SearchString,
            OrderBy = orderings ?? Array.Empty<string>()
        };

        if (!Context.AllColumnsChecked)
        {
            filter.AdvancedSearch = new()
            {
                Fields = Context.SearchFields,
                Keyword = filter.Keyword
            };
            filter.Keyword = null;
        }

        return filter;
    }

    private BaseFilter GetBaseFilter()
    {
        var filter = new BaseFilter
        {
            Keyword = SearchString,
        };

        if (!Context.AllColumnsChecked)
        {
            filter.AdvancedSearch = new()
            {
                Fields = Context.SearchFields,
                Keyword = filter.Keyword
            };
            filter.Keyword = null;
        }

        return filter;
    }

    private void InvokeEditPage(TEntity entity)
    {
        var url = Context.EditNavigateTo?.Invoke(entity);

        if (url != null)
            NavigationManager.NavigateTo(url);
    }

    private void InvokeCreatePage()
    {
        var url = Context.CreateNavigateTo?.Invoke();

        Console.WriteLine(url);

        if (url != null)
            NavigationManager.NavigateTo(url);
    }

    private async Task InvokeModal(TEntity? entity = default)
    {
        await LoadingService.ToggleLoaderVisibility(true);
        
        var isCreate = entity is null;
        var canSaveEntity = entity != null && CanUpdateEntity(entity) || (isCreate && (_canCreate || _canUpdate));
        
        var parameters = new DialogParameters()
        {
            {nameof(AddEditModal<TRequest>.EditFormContent), EditFormContent},
            {nameof(AddEditModal<TRequest>.WizardStep1), WizardStep1},
            {nameof(AddEditModal<TRequest>.WizardStep1Content), WizardStep1Content},
            {nameof(AddEditModal<TRequest>.WizardStep2Content), WizardStep2Content},
            {nameof(AddEditModal<TRequest>.WizardStep3Content), WizardStep3Content},
            {nameof(AddEditModal<TRequest>.WizardStep4Content), WizardStep4Content},
            {nameof(AddEditModal<TRequest>.WizardStep5Content), WizardStep5Content},
            {nameof(AddEditModal<TRequest>.WizardStep6Content), WizardStep6Content},
            {nameof(AddEditModal<TRequest>.WizardStep7Content), WizardStep7Content},
            {nameof(AddEditModal<TRequest>.WizardStep8Content), WizardStep8Content},
            {nameof(AddEditModal<TRequest>.OnInitializedFunc), Context.EditFormInitializedFunc},
            {nameof(AddEditModal<TRequest>.EntityName), Context.EntityName},
            {nameof(AddEditModal<TRequest>.IsWizard), IsWizard},
            {nameof(AddEditModal<TRequest>.IsFullScreenModal), FullScreenModal},
            {nameof(AddEditModal<TRequest>.WizardSteps), WizardSteps},
            {nameof(AddEditModal<TRequest>.CanSaveEntity), canSaveEntity},
        };

        TRequest requestModel;

        if (isCreate)
        {
            _ = Context.CreateFunc ?? throw new InvalidOperationException("CreateFunc can't be null!");
            parameters.Add(nameof(AddEditModal<TRequest>.SaveFunc), Context.CreateFunc);

            requestModel =
                Context.GetDefaultsFunc is not null
                && await ServiceHelper.ExecuteCallGuardedAsync(
                        () => Context.GetDefaultsFunc(), Snackbar,
                        Logger)
                    is { } defaultsResult
                    ? defaultsResult
                    : new TRequest();
        }
        else
        {
            _ = Context.IdFunc ?? throw new InvalidOperationException("IdFunc can't be null!");
            var id = Context.IdFunc(entity!);
            parameters.Add(nameof(AddEditModal<TRequest>.Id), id);

            if (Context.UpdateFunc == null)
            {
                parameters.Add(nameof(AddEditModal<TRequest>.SaveFunc), null);
            }
            else
            {
                Task SaveFunc(TRequest entity) => Context.UpdateFunc(id, entity);
                parameters.Add(nameof(AddEditModal<TRequest>.SaveFunc), (Func<TRequest, Task>) SaveFunc);
            }

            requestModel =
                Context.GetDetailsFunc is not null
                && await ServiceHelper.ExecuteCallGuardedAsync(
                        () => Context.GetDetailsFunc(id!),
                        Snackbar,
                        Logger)
                    is { } detailsResult
                    ? detailsResult
                    : entity!.Adapt<TRequest>();
        }

        parameters.Add(nameof(AddEditModal<TRequest>.RequestModel), requestModel);

        var options = new DialogOptions {CloseButton = true, MaxWidth = ModalWidth, FullScreen = FullScreenModal, FullWidth = true, DisableBackdropClick = true};

        var dialog = await DialogService.ShowAsync<AddEditModal<TRequest>>(string.Empty, parameters, options);
        
        await LoadingService.ToggleLoaderVisibility(false);
        
        Context.SetAddEditModalRef(dialog);

        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await ReloadDataAsync();
        }


    }

    private async Task Delete(TEntity entity)
    {
        _ = Context.IdFunc ?? throw new InvalidOperationException("IdFunc can't be null!");
        var id = Context.IdFunc(entity);

        string deleteContent = L["You're sure you want to delete {0} with id '{1}'?"];
        var parameters = new DialogParameters
        {
            {nameof(DeleteConfirmation.ContentText), string.Format(deleteContent, Context.EntityName, id)}
        };
        var options = new DialogOptions
            {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true};
        var dialog = await DialogService.ShowAsync<DeleteConfirmation>(L["Delete"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            _ = Context.DeleteFunc ?? throw new InvalidOperationException("DeleteFunc can't be null!");

            await ServiceHelper.ExecuteCallGuardedAsync(
                () => Context.DeleteFunc(id),
                Snackbar,
                Logger);

            await ReloadDataAsync();
        }
    }
}