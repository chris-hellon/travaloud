namespace Travaloud.Application.Catalog.Partners.Commands;

public class CreatePartnerContactRequest : IRequest<DefaultIdType>
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string ContactNumber { get; set; } = default!;
    public string EmailAddress { get; set; } = default!;
}