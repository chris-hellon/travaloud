using Travaloud.Application.Catalog.Galleries.Commands;

namespace Travaloud.Application.Catalog.Galleries.Validation;

public class CreateGalleryRequestValidator : AbstractValidator<CreateGalleryRequest>
{
    public CreateGalleryRequestValidator()
    {
        RuleFor(p => p.Title)
            .NotEmpty();

        RuleFor(p => p.Description)
            .NotEmpty();

        RuleFor(b => b.GalleryImages)
            .NotEmpty()
            .ForEach(itemRule => itemRule.SetValidator(new GalleryImageValidator()));
    }
}

public class GalleryImageValidator : CustomValidator<GalleryImageRequest>
{
    public GalleryImageValidator()
    {
        RuleFor(p => p.Image)
            .SetNonNullableValidator(new FileUploadRequestValidator());
    }
}