using Travaloud.Application.Catalog.Services.Dto;

namespace Travaloud.Application.Catalog.ServicesEnquiries.Dto;

public class ServiceEnquiryDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType ServiceId { get; set; }
    public bool ResponseSent { get; set; }
    public DateTime CreatedOn { get; set; }
    public ServiceDto Service { get; set; } = default!;
}