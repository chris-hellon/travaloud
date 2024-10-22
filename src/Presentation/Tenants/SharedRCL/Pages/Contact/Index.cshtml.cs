using Travaloud.Application.Catalog.Interfaces;

namespace Travaloud.Tenants.SharedRCL.Pages.Contact;

public class IndexModel : ContactPageModel
{
    public IndexModel(IGeneralEnquiriesService generalEnquiriesService) : base(generalEnquiriesService)
    {
        
    }
    
    public override string MetaKeywords(string? overrideValue = null)
    {
        return "Vietnam Backpacker Hostels, contact us, customer support, travel assistance, Vietnam travel, backpacker travel";
    }

    public override string MetaDescription(string? overrideValue = null)
    {
        return "Get in touch with Vietnam Backpacker Hostels for all your travel needs. Our friendly team is always ready to help you plan your trip and answer any questions you may have.";
    }
}