namespace Travaloud.Application.Cloudbeds.Commands;

public class CreateReservationCustomFieldRequest
{
    [JsonProperty("fieldName")] public string FieldName { get; set; } = default!;

    [JsonProperty("fieldValue")] public string FieldValue { get; set; } = default!;
}