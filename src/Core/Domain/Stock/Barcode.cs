namespace Travaloud.Domain.Stock;

public abstract class Barcode(string barcodeNumber, string? barcodeFormat, string json) 
    : AuditableEntity, IAggregateRoot
{
    [MaxLength(1024)]
    private string BarcodeNumber { get; set; } = barcodeNumber;
    
    [MaxLength(200)]
    private string? BarcodeFormat { get; set; } = barcodeFormat;
    
    private string Json { get; set; } = json;

    public Barcode Update(string? barcodeNumber, string? barcodeFormat, string? json)
    {
        if (barcodeNumber is not null && !string.Equals(BarcodeNumber, barcodeNumber))
            BarcodeNumber = barcodeNumber;

        if (barcodeFormat is not null && !string.Equals(BarcodeFormat, barcodeFormat))
            BarcodeFormat = barcodeFormat;

        if (json is not null && !string.Equals(Json, json))
            Json = json;

        return this;
    }
}