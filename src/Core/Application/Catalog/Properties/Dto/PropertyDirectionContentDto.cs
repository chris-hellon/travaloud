namespace Travaloud.Application.Catalog.Properties.Dto;

public class PropertyDirectionContentDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType PropertyDirectionId { get; set; }
    public string Body { get; set; } = default!;
    public string? Style { get; set; }
}