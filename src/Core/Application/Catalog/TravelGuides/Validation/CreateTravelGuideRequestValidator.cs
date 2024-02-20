using Travaloud.Application.Catalog.TravelGuides.Commands;
using Travaloud.Application.Catalog.TravelGuides.Specification;
using Travaloud.Domain.Catalog.TravelGuides;

namespace Travaloud.Application.Catalog.TravelGuides.Validation;

public class CreateTravelGuideRequestValidator : CustomValidator<CreateTravelGuideRequest>
{
    public CreateTravelGuideRequestValidator(IRepositoryFactory<TravelGuide> repo, IStringLocalizer<CreateTravelGuideRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MaximumLength(1024)
            .MustAsync(async (name, ct) => await repo.SingleOrDefaultAsync(new TravelGuideByTitleSpec(name), ct) is null)
            .WithMessage((_, name) => string.Format(localizer["service.alreadyexists"], name));

        RuleFor(p => p.SubTitle)
            .NotEmpty();
        
        RuleFor(p => p.ShortDescription)
            .NotEmpty();
        
        RuleFor(p => p.Description)
            .NotEmpty();
        
        RuleFor(p => p.Image)
            .SetNonNullableValidator(new FileUploadRequestValidator());
    }
}