namespace Travaloud.Application.Catalog.Services.Dto;

public class ServiceDetailsDto
{
    public DefaultIdType Id { get; set; }
    public string Title { get; set; } = default!;
    public string? SubTitle { get; set; }
    public string Description { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public string? BodyHtml { get; set; }
    public string? IconClass { get; set; }
    public string FriendlyUrl => Title.UrlFriendly();

    public IList<ServiceFieldDto> ServiceFields { get; set; } = default!;
}