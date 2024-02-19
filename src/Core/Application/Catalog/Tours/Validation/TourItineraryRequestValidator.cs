using Travaloud.Application.Catalog.Tours.Commands;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Validation;

public class TourItineraryRequestValidator : CustomValidator<TourItineraryRequest>
{
    public TourItineraryRequestValidator(IRepositoryFactory<TourItinerary> tourItineraryRepo, IStringLocalizer<TourItineraryRequestValidator> localizer)
    {
        RuleFor(p => p.Header)
            .NotEmpty()
            .MaximumLength(1024);

        RuleFor(p => new { p.Header, p.Id, p.TourId })
            .MustAsync(async (tourItinerary, ct) => await Validate(tourItinerary.Header, tourItinerary.TourId, tourItinerary.Id, tourItineraryRepo, ct))
            .WithMessage((_, name) => string.Format(localizer["tourItinerary.alreadyexists"], name));

        RuleFor(p => p.Sections)
            .NotNull()
            .NotEmpty();
    }

    private static async Task<bool> Validate(string header, DefaultIdType tourId, DefaultIdType? id, IRepositoryFactory<TourItinerary> repo, CancellationToken ct)
    {
        var response = await repo.SingleOrDefaultAsync(new TourItineraryByNameSpec(header, tourId), ct);

        if (id.HasValue)
        {
            return response is not TourItinerary existingTourDate || existingTourDate.Id == id;
        }
        else
        {
            return response is null;
        }
    }
}