using Travaloud.Application.Catalog.ServicesEnquiries.Commands;

namespace Travaloud.Application.Catalog.ServicesEnquiries.Validation;

public class CreateServiceEnquiryRequestValidator : CustomValidator<CreateServiceEnquiryRequest>
{
    public CreateServiceEnquiryRequestValidator()
    {
        RuleFor(p => p.Fields)
            .NotEmpty();

        RuleFor(p => p.ServiceId)
            .NotEmpty();
    }
}