namespace Travaloud.Application.Cloudbeds.Dto;

public class PropertyCurrencyDto
{
    [JsonProperty("currencyCode")] public string CurrencyCode { get; set; } = default!;

    [JsonProperty("currencySymbol")] public string CurrencySymbol { get; set; } = default!;

    [JsonProperty("currencyPosition")] public string CurrencyPosition { get; set; } = default!;
}