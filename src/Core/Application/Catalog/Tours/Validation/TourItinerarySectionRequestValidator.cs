using Travaloud.Application.Catalog.Tours.Commands;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Validation;

public class TourItinerarySectionRequestValidator : CustomValidator<TourItinerarySectionRequest>
{
    public TourItinerarySectionRequestValidator(IRepositoryFactory<TourItinerarySection> tourItineraryRepo, IStringLocalizer<TourItinerarySectionRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MaximumLength(1024);

        RuleFor(p => new { p.Id, p.TourItineraryId, p.Title })
            .MustAsync(async (tourItinerarySection, ct) => await Validate(tourItineraryRepo, tourItinerarySection.Title, tourItinerarySection.TourItineraryId, tourItinerarySection.Id, ct))
            .WithMessage((_, name) => string.Format(localizer["tourItinerarySection.alreadyexists"], name));
    }

    private static async Task<bool> Validate(IRepositoryFactory<TourItinerarySection> repo, string title, DefaultIdType? tourItineraryId, DefaultIdType? id, CancellationToken ct)
    {
        var response = await repo.SingleOrDefaultAsync(new TourItinerarySectionByNameSpec(title, tourItineraryId), ct);

        if (id.HasValue)
        {
            return response is not TourItinerarySection existingTourDate || existingTourDate.Id == id;
        }
        else
        {
            return response is null;
        }
    }
}