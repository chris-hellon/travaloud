namespace Travaloud.Application.Catalog.Tours.Commands;

public class DeleteTourDateRequest : IRequest<DefaultIdType>
{
    public DeleteTourDateRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}