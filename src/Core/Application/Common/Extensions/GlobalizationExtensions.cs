using System.Globalization;

namespace Travaloud.Application.Common.Extensions;

public static class GlobalizationExtensions
{
    public static string? CountryToTwoLetterCode(this string countryName)
    {
        // Get all the cultures
        var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

        foreach (var culture in cultures)
        {
            var region = new RegionInfo(culture.Name);
            if (region.EnglishName.Contains(countryName, StringComparison.OrdinalIgnoreCase))
            {
                return region.TwoLetterISORegionName;
            }
        }

        return null;
    }
}