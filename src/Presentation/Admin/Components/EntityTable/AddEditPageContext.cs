namespace Travaloud.Admin.Components.EntityTable;

public class AddEditPageContext<TId, TEntity, TRequest>
{
    public Func<TId, TRequest, Task> UpdateFunc { get; set; } = default!;

    public Func<TRequest, Task> CreateFunc { get; set; } = default!;

    public Func<Task>? OnInitializedFunc { get; set; }

    public Func<TId, Task<TRequest>> GetDetailsFunc { get; }

    public string EntityName { get; set; } = default!;

    public string EntityNamePlural { get; }

    public TId? Id { get; set; }

    public bool IsCreate => Id is null;

    private TRequest? _addEditPageModel;
    public TRequest RequestModel => _addEditPageModel != null ? _addEditPageModel : default!;

    internal void SetAddEditPageRequestModel(TRequest model) =>
        _addEditPageModel = model;

    public AddEditPageContext(
        TId? id,
        string entityName,
        string entityNamePlural,
        Func<TRequest, Task> createFunc,
        Func<TId, TRequest, Task> updateFunc,
        Func<TId, Task<TRequest>> getDetailsFunc,
        Func<Task>? onInitializedFunc = null)
    {
            Id = id;
            EntityName = entityName;
            EntityNamePlural = entityNamePlural;
            OnInitializedFunc = onInitializedFunc;
            UpdateFunc = updateFunc;
            CreateFunc = createFunc;
            GetDetailsFunc = getDetailsFunc;
        }
}