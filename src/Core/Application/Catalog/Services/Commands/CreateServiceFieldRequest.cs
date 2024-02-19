namespace Travaloud.Application.Catalog.Services.Commands;

public class CreateServiceFieldRequest : IRequest<DefaultIdType>
{
    public string Label { get; set; } = default!;
    public string FieldType { get; set; } = default!;
    public int Width { get; set; }
    public string? SelectOptions { get; set; }
    public bool IsRequired { get; set; }
}