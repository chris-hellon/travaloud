namespace Travaloud.Application.Catalog.PageModals.Dto;

public class PageModalDetailsDto
{
    public DefaultIdType Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImagePath { get; set; }
    public string? CallToAction { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public IEnumerable<PageModalLookupDto> PageModalLookups { get; set; }
}