using Travaloud.Application.Common.Interfaces;

namespace Travaloud.Infrastructure.Common.Services;

public class LoadingService : ILoadingService
{
    public bool LoaderVisible {get;set;}
    public Func<Task> OnLoaderVisibilityChanged { get; set; } = default!;
    public async Task ToggleLoaderVisibility(bool isVisible)
    {
        LoaderVisible = isVisible;
        await OnLoaderVisibilityChanged.Invoke();
    }
}