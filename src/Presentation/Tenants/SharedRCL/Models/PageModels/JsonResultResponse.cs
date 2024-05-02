using System.Text.Json.Serialization;

namespace Travaloud.Tenants.SharedRCL.Models.PageModels;

public class JsonResultResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("value")]
    public object? Value { get; set; }

    [JsonPropertyName("modalMessage")]
    public string? ModalMessage { get; set; }

    public JsonResultResponse(bool success, object? value = null, string? modalMessage = null)
    {
        Success = success;
        Value = value;
        ModalMessage = modalMessage;
    }
}
