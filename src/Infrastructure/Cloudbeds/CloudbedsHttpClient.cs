using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Travaloud.Application.Cloudbeds;

namespace Travaloud.Infrastructure.Cloudbeds;

public class CloudbedsHttpClient : ICloudbedsHttpClient
{
    private readonly HttpClient _httpClient;

    public CloudbedsHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await _httpClient.SendAsync(request, cancellationToken);
    }
    
    public Uri BuildApiUrl<TRequest>(string path, TRequest request)
    {
        var baseUrl = new UriBuilder(_httpClient.BaseAddress!)
        {
            Path = CombinePaths(_httpClient.BaseAddress!.AbsolutePath, path)
        };

        var queryProperties = typeof(TRequest).GetProperties()
            .Where(p => !Attribute.IsDefined(p, typeof(JsonIgnoreAttribute)))
            .ToList();
        var queryBuilder = new StringBuilder();

        foreach (var property in queryProperties)
        {
            var jsonPropertyAttribute = property.GetCustomAttribute<JsonPropertyAttribute>();
            var propertyName = jsonPropertyAttribute?.PropertyName ?? property.Name;

            var value = property.GetValue(request);
            if (value == null) continue;
            if (queryBuilder.Length > 0)
                queryBuilder.Append('&');

            queryBuilder.Append(Uri.EscapeDataString(propertyName));
            queryBuilder.Append('=');
            queryBuilder.Append(Uri.EscapeDataString(value.ToString()!));
        }

        baseUrl.Query = queryBuilder.ToString();

        return baseUrl.Uri;
    }
    
    private static string CombinePaths(string basePath, string relativePath)
    {
        return string.IsNullOrEmpty(basePath) ? relativePath.TrimStart('/') : $"{basePath.TrimEnd('/')}/{relativePath.TrimStart('/')}";
    }
}