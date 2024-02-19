namespace Travaloud.Application.Catalog.Enquiries.Dto;

public class GeneralEnquiryDto
{
    public DefaultIdType Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string ContactNumber { get; set; } = default!;
    public string Message { get; set; } = default!;
    public bool ResponseSent { get; set; }
    public DateTime CreatedOn { get; set; }
}