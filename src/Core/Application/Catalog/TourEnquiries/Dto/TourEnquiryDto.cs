using Travaloud.Application.Catalog.Tours.Dto;

namespace Travaloud.Application.Catalog.TourEnquiries.Dto;

public class TourEnquiryDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType TourId { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string ContactNumber { get; set; } = default!;
    public DateTime RequestedDate { get; set; } = default!;
    public int NumberOfPeople { get; set; } = default!;
    public string? AdditionalInformation { get; set; }
    public bool ResponseSent { get; set; }
    public DateTime CreatedOn { get; set; }
    public TourDto Tour { get; set; } = default!;
}