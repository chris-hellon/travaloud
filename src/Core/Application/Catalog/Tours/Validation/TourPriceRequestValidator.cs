using Travaloud.Application.Catalog.Tours.Commands;

namespace Travaloud.Application.Catalog.Tours.Validation;

public class TourPriceRequestValidator : CustomValidator<TourPriceRequest>
{
    public TourPriceRequestValidator()
    {
        RuleFor(p => p.Price)
            .NotNull()
            .NotEmpty()
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x)
            .Must(request => request.DayDuration != null || request.NightDuration != null || request.HourDuration != null)
            .WithMessage("At least one duration (Day Duration, Night Duration or Hour Duration) must be provided");
    }
}