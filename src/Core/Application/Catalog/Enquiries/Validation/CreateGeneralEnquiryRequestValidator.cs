using Travaloud.Application.Catalog.Enquiries.Commands;

namespace Travaloud.Application.Catalog.Enquiries.Validation;

public class CreateGeneralEnquiryRequestValidator : CustomValidator<CreateGeneralEnquiryRequest>
{
    public CreateGeneralEnquiryRequestValidator()
    {
        RuleFor(p => p.Name)
            .MaximumLength(200)
            .NotEmpty();

        RuleFor(p => p.ContactNumber)
            .MaximumLength(200)
            .NotEmpty();

        RuleFor(p => p.Email)
            .MaximumLength(200)
            .NotEmpty();

        RuleFor(p => p.Message)
            .NotEmpty();
    }
}