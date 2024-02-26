using Travaloud.Application.Catalog.Services.Commands;

namespace Travaloud.Application.Catalog.Services.Dto;

public class ServiceFieldDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType ServiceId { get; set; }
    public string Label { get; set; } = default!;
    public string FieldType { get; set; } = default!;
    public string? Value { get; set; }
    public int Width { get; set; }
    public string? SelectOptions { get; set; }
    public ICollection<ServiceFieldSelectOption>? SelectOptionsParsed => SelectOptions != null ? JsonConvert.DeserializeObject<List<ServiceFieldSelectOption>>(SelectOptions) : default(ICollection<ServiceFieldSelectOption>?);

    public int SortOrder { get; set; }
    public bool IsRequired { get; set; }
}