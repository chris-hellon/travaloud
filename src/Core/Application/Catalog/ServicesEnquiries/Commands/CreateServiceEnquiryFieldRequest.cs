namespace Travaloud.Application.Catalog.ServicesEnquiries.Commands;

public class CreateServiceEnquiryFieldRequest : IRequest<DefaultIdType>
{
    public string Field { get; set; } = default!;
    public string Value { get; set; } = default!;
}