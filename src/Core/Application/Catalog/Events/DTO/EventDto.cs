namespace Travaloud.Application.Catalog.Events.DTO;

public class EventDto
{
    public DefaultIdType Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public string? ImagePath { get; set; }
    public string? PageTitle { get; set; }
    public string? PageSubTitle { get; set; }
    public string BackgroundColor { get; set; } = default!;
    public DefaultIdType? PropertyId { get; private set; }
    public string FriendlyUrl => Name.UrlFriendly();
}