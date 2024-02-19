using Travaloud.Application.Catalog.TourEnquiries.Commands;

namespace Travaloud.Application.Catalog.TourEnquiries.Validation;

public class UpdateTourEnquiryRequestValidator : CustomValidator<UpdateTourEnquiryRequest>
{
    public UpdateTourEnquiryRequestValidator()
    {
        RuleFor(p => p.Name)
            .MaximumLength(200)
            .NotEmpty();

        RuleFor(p => p.ContactNumber)
            .MaximumLength(100)
            .NotEmpty();

        RuleFor(p => p.Email)
            .MaximumLength(200)
            .NotEmpty();

        RuleFor(p => p.NumberOfPeople)
            .NotEmpty();

        RuleFor(p => p.RequestedDate)
            .NotEmpty();

        RuleFor(p => p.TourId)
            .NotEmpty();
    }
}