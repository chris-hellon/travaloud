using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Travaloud.Application.Cloudbeds;

namespace Travaloud.Infrastructure.Cloudbeds;

public static class Startup
{
    public static IHttpClientBuilder AddCloudbedsHttpClient(this IServiceCollection services, IConfiguration config)
    {
        return services.AddHttpClient<ICloudbedsHttpClient, CloudbedsHttpClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.cloudbeds.com/api/v1.1/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // Add other HttpClient configurations here if needed
        });
    }
}