using Travaloud.Application.Catalog.TravelGuides.Commands;
using Travaloud.Application.Catalog.TravelGuides.Specification;
using Travaloud.Domain.Catalog.TravelGuides;

namespace Travaloud.Application.Catalog.TravelGuides.Validation;

public class UpdateTravelGuideRequestValidator : CustomValidator<UpdateTravelGuideRequest>
{
    public UpdateTravelGuideRequestValidator(IRepositoryFactory<TravelGuide> repo, IStringLocalizer<UpdateTravelGuideRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MaximumLength(1024)
            .MustAsync(async (travelGuide, name, ct) =>
                await repo.FirstOrDefaultAsync(new TravelGuideByTitleSpec(name), ct)
                    is not TravelGuide existingTravelGuide || existingTravelGuide.Id == travelGuide.Id)
            .WithMessage((_, name) => string.Format(localizer["travelGuide.alreadyexists"], name));

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