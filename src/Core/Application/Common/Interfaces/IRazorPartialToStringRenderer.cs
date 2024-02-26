namespace Travaloud.Application.Common.Interfaces;

public interface IRazorPartialToStringRenderer
{
    Task<string> RenderPartialToStringAsync<TModel>(string partialName, TModel model);
}