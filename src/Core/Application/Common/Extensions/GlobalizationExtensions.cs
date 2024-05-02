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
    
    public static string TwoLetterCodeToCountry(this string twoLetterCode)
    {
        if (twoLetterCode.Length > 2)
            return twoLetterCode;
        
        // Get all the cultures
        var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

        foreach (var culture in cultures)
        {
            var region = new RegionInfo(culture.Name);
            if (region.TwoLetterISORegionName.Equals(twoLetterCode, StringComparison.OrdinalIgnoreCase))
            {
                return region.EnglishName;
            }
        }

        return "";
    }

    public static string GenderMatch(this string? gender)
    {
        if (string.IsNullOrEmpty(gender))
            return "";
        
        return gender switch
        {
            "M" => "Male",
            "F" => "Female",
            _ => gender
        };
    }
}