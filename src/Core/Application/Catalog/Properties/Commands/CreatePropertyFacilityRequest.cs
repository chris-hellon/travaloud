namespace Travaloud.Application.Catalog.Properties.Commands;

public class CreatePropertyFacilityRequest : IRequest<DefaultIdType>
{
    public string Title { get; set; } = default!;
}