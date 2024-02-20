using Travaloud.Application.Catalog.Galleries.Commands;
using Travaloud.Application.Catalog.Galleries.Specification;
using Travaloud.Domain.Catalog.Galleries;

namespace Travaloud.Application.Catalog.Galleries.Validation;

public class UpdateGalleryRequestValidator : CustomValidator<UpdateGalleryRequest>
{
    public UpdateGalleryRequestValidator(IRepositoryFactory<Gallery> repo, IStringLocalizer<UpdateGalleryRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MaximumLength(1024)
            .MustAsync(async (gallery, name, ct) =>
                await repo.FirstOrDefaultAsync(new GalleryByTitleSpec(name), ct)
                    is not Gallery existingGallery || existingGallery.Id == gallery.Id)
            .WithMessage((_, name) => string.Format(localizer["Gallery already exists."], name));
        
        RuleFor(p => p.Description)
            .NotEmpty();
        
        RuleFor(b => b.GalleryImages)
            .NotEmpty()
            .ForEach(itemRule => itemRule.SetValidator(new GalleryImageValidator()));
    }
}