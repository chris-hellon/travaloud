namespace Travaloud.Admin.Components.EntityTable;

public interface INestedAddEditModal<TRequest>
{
    TRequest RequestModel { get; }
    bool IsCreate { get; }
}