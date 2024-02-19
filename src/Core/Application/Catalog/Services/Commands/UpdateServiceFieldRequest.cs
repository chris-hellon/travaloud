namespace Travaloud.Application.Catalog.Services.Commands;

public class UpdateServiceFieldRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public string Label { get; set; } = default!;
    public string FieldType { get; set; } = default!;
    public int Width { get; set; }
    public string? SelectOptions { get; set; }
    public List<ServiceFieldSelectOption>? SelectOptionsParsed { get; set; }
    public bool IsRequired { get; set; }
}

public class ServiceFieldSelectOption
{
    public string? Key { get; set; }
    public string Value { get; set; } = default!;
    public string? ListName { get; set; }

    public ServiceFieldSelectOption()
    {
    }

    public ServiceFieldSelectOption(string key, string value, string listName)
    {
        Key = key;
        Value = value;
        ListName = listName;
    }
}