namespace Travaloud.Application.Cloudbeds.Responses;

public class CreateReservationPaymentResponse
{
    [JsonProperty("success")]  public bool Success { get; set; }
    [JsonProperty("paymentID")] public int? PaymentId { get; set; }
    [JsonProperty("transactionID")] public string? TransactionId { get; set; }
}