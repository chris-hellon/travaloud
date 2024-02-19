using Travaloud.Application.Catalog.ServicesEnquiries.Commands;

namespace Travaloud.Application.Catalog.ServicesEnquiries.Validation;

public class UpdateServiceEnquiryRequestValidator : CustomValidator<UpdateServiceEnquiryRequest>
{
    public UpdateServiceEnquiryRequestValidator()
    {
        RuleFor(p => p.Fields)
            .NotEmpty();

        RuleFor(p => p.ServiceId)
            .NotEmpty();
    }
}