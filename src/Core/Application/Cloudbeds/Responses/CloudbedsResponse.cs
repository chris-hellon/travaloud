namespace Travaloud.Application.Cloudbeds.Responses;

public class CloudbedsResponse<T>
{
    [JsonProperty("success")] public bool Success { get; set; }

    [JsonProperty("data")] public T? Data { get; set; }

    [JsonProperty("count")] public int Count { get; set; }

    [JsonProperty("total")] public int Total { get; set; }

    [JsonProperty("message")] public string? Message { get; set; }
}