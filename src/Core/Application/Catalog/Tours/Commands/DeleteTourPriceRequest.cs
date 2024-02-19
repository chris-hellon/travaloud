namespace Travaloud.Application.Catalog.Tours.Commands;

public class DeleteTourPriceRequest : IRequest<DefaultIdType>
{
    public DeleteTourPriceRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}