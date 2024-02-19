namespace Travaloud.Application.Catalog.Properties.Commands;

public class CreatePropertyDirectionRequest : IRequest<DefaultIdType>
{
    public string Title { get; set; } = default!;
    public IList<CreatePropertyDirectionContentRequest> Content { get; set; } = default!;
}