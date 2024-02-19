namespace Travaloud.Application.Catalog.Properties.Dto;

public class PropertyRoomDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType PropertyId { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public string? ImagePath { get; set; }
    public string? PageTitle { get; set; }
    public string? PageSubTitle { get; set; }
}