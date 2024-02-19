namespace Travaloud.Application.Catalog.Partners.Dto;

public class PartnerDto
{
    public DefaultIdType Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string Address { get; set; } = default!;
    public string City { get; set; } = default!;
    public string PrimaryContactName { get; set; } = default!;
    public string PrimaryContactNumber { get; set; } = default!;
    public string PrimaryEmailAddress { get; set; } = default!;
}