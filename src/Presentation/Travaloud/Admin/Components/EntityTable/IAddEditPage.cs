namespace Travaloud.Admin.Components.EntityTable;

public interface IAddEditPage<TEntity, TId, TRequest>
{
    TEntity Entity { get; }
    TId Id { get; set; }
    TRequest RequestModel { get; }
    void ForceRender();
    bool Validate(object request);
}