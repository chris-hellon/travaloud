namespace Travaloud.Application.Catalog.ServicesEnquiries.Commands;

public class UpdateServiceEnquiryFieldRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType ServiceEnquiryId { get; set; }
    public string Field { get; set; } = default!;
    public string Value { get; set; } = default!;
}