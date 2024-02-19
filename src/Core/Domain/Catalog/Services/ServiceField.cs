namespace Travaloud.Domain.Catalog.Services;

public class ServiceField : AuditableEntity, IAggregateRoot
{
    public DefaultIdType ServiceId { get; set; }
    public string Label { get; set; } = default!;
    public string FieldType { get; set; } = default!;
    public int Width { get; set; }
    public string? SelectOptions { get; set; }
    public bool IsRequired { get; set; }

    public ServiceField()
    {
    }

    public ServiceField(string label, string fieldType, int width, string? selectOptions, bool isRequired)
    {
        Label = label;
        FieldType = fieldType;
        Width = width;
        SelectOptions = selectOptions;
        IsRequired = isRequired;
    }

    public ServiceField Update(string? label, string? fieldType, int? width, string? selectOptions, bool? isRequired)
    {
        if (label is not null && Label?.Equals(label) is not true) Label = label;
        if (fieldType is not null && FieldType?.Equals(fieldType) is not true) FieldType = fieldType;
        if (selectOptions is not null && SelectOptions?.Equals(selectOptions) is not true) SelectOptions = selectOptions;
        if (isRequired.HasValue && IsRequired != isRequired) IsRequired = isRequired.Value;
        if (width.HasValue && Width != width) Width = width.Value;
        return this;
    }
}