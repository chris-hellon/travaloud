namespace Travaloud.Application.Catalog.Properties.Commands;

public class PropertyDirectionRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType PropertyId { get; set; }
    public string Title { get; set; } = default!;
    public IList<PropertyDirectionContentRequest> Content { get; set; } = default!;
}