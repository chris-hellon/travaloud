namespace Travaloud.Application.Catalog.Properties.Dto;

public class PropertyDirectionDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType PropertyId { get; set; }
    public string Title { get; set; } = default!;

    public IList<PropertyDirectionContentDto> Content { get; set; } = default!;
}