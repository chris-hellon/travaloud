namespace Travaloud.Application.Common.Interfaces;

public interface ILoadingService : IScopedService
{
    bool LoaderVisible {get;set;}
    Func<Task> OnLoaderVisibilityChanged {get;set;}
    Task ToggleLoaderVisibility(bool isVisible);
}