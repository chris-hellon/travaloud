namespace Travaloud.Application.Catalog.Properties.Commands;

public class UpdatePropertyDirectionContentRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public string Body { get; set; } = default!;
    public string? Style { get; set; }
}