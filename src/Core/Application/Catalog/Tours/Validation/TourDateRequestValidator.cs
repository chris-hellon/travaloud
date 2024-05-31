using Travaloud.Application.Catalog.TourDates.Specification;
using Travaloud.Application.Catalog.Tours.Commands;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Validation;

public class TourDateRequestValidator : CustomValidator<TourDateRequest>
{
    public TourDateRequestValidator(IRepositoryFactory<TourDate> repo, IStringLocalizer<TourDateRequestValidator> localizer)
    {
        RuleFor(p => p.StartDate).NotEmpty();
        RuleFor(p => p.StartTime).NotEmpty();
        RuleFor(p => p.TourPriceId).NotEmpty();

        RuleFor(p => new { p.StartDate, p.EndDate, p.TourId, p.Id })
            .MustAsync(async (tourDate, ct) => await Validate(repo, tourDate.StartDate, tourDate.EndDate, tourDate.TourId, tourDate.Id, ct))
            .WithMessage((_, name) => string.Format(localizer["tourDate.alreadyexists"], name));
    }

    private static async Task<bool> Validate(IRepositoryFactory<TourDate> repo, DateTime? startDate, DateTime? endDate, DefaultIdType tourId, DefaultIdType? id, CancellationToken ct)
    {
        var tourDateResponse = await repo.SingleOrDefaultAsync(new TourDateByDatesSpec(startDate, endDate, tourId), ct);

        if (id.HasValue)
        {
            return tourDateResponse is not TourDate existingTourDate || existingTourDate.Id == id;
        }
        else
        {
            return tourDateResponse is null;
        }
    }
}