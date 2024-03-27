namespace Travaloud.Application.PaymentProcessing;

public class StripeSettings
{
    public string ApiPublishKey { get; set; } = default!;
    public string ApiSecretKey { get; set; } = default!;
}