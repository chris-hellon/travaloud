namespace Travaloud.Application.Cloudbeds;

public interface ICloudbedsHttpClient
{
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);

    Uri BuildApiUrl<TRequest>(string path, TRequest request);
}