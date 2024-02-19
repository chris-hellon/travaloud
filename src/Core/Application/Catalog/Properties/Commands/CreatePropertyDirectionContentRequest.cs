namespace Travaloud.Application.Catalog.Properties.Commands;

public class CreatePropertyDirectionContentRequest : IRequest<DefaultIdType>
{
    public string Body { get; set; } = default!;
    public string? Style { get; set; }
}