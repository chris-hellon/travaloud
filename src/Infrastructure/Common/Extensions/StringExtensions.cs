using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Travaloud.Infrastructure.Common.Extensions;

public static class StringExtensions
{
    public static string UrlFriendly(this string text, int maxLength = 0)
    {
        // Return empty value if text is null
        if (text == null) return string.Empty;

        if (text.Contains('&'))
            text = text.Replace("&", "and");

        var normalizedString = text
            .ToLowerInvariant()
            .Normalize(NormalizationForm.FormD);

        var stringBuilder = new StringBuilder();
        var stringLength = normalizedString.Length;
        var prevdash = false;
        var trueLength = 0;

        char c;

        for (var i = 0; i < stringLength; i++)
        {
            c = normalizedString[i];

            switch (CharUnicodeInfo.GetUnicodeCategory(c))
            {
                // Check if the character is a letter or a digit if the character is a
                // international character remap it to an ascii valid character
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.DecimalDigitNumber:
                    if (c < 128)
                        stringBuilder.Append(c);
                    else
                        stringBuilder.Append(RemapInternationalCharToAscii(c));

                    prevdash = false;
                    trueLength = stringBuilder.Length;
                    break;

                // Check if the character is to be replaced by a hyphen but only if the last character wasn't
                case UnicodeCategory.SpaceSeparator:
                case UnicodeCategory.ConnectorPunctuation:
                case UnicodeCategory.DashPunctuation:
                case UnicodeCategory.OtherPunctuation:
                case UnicodeCategory.MathSymbol:
                    if (!prevdash)
                    {
                        stringBuilder.Append('-');
                        prevdash = true;
                        trueLength = stringBuilder.Length;
                    }

                    break;
            }

            // If we are at max length, stop parsing
            if (maxLength > 0 && trueLength >= maxLength)
                break;
        }

        // Trim excess hyphens
        var result = stringBuilder.ToString().Trim('-');

        // Remove any excess character to meet maxlength criteria
        return maxLength <= 0 || result.Length <= maxLength ? result : result.Substring(0, maxLength);
    }

    private static string RemapInternationalCharToAscii(char c)
    {
        var s = c.ToString().ToLowerInvariant();
        if ("àåáâäãåą".Contains(s))
        {
            return "a";
        }
        else if ("èéêëę".Contains(s))
        {
            return "e";
        }
        else if ("ìíîïı".Contains(s))
        {
            return "i";
        }
        else if ("òóôõöøőð".Contains(s))
        {
            return "o";
        }
        else if ("ùúûüŭů".Contains(s))
        {
            return "u";
        }
        else if ("çćčĉ".Contains(s))
        {
            return "c";
        }
        else if ("żźž".Contains(s))
        {
            return "z";
        }
        else if ("śşšŝ".Contains(s))
        {
            return "s";
        }
        else if ("ñń".Contains(s))
        {
            return "n";
        }
        else if ("ýÿ".Contains(s))
        {
            return "y";
        }
        else if ("ğĝ".Contains(s))
        {
            return "g";
        }
        else if (c == 'ř')
        {
            return "r";
        }
        else if (c == 'ł')
        {
            return "l";
        }
        else if (c == 'đ')
        {
            return "d";
        }
        else if (c == 'ß')
        {
            return "ss";
        }
        else if (c == 'þ')
        {
            return "th";
        }
        else if (c == 'ĥ')
        {
            return "h";
        }
        else if (c == 'ĵ')
        {
            return "j";
        }
        else
        {
            return string.Empty;
        }
    }
    
    public static string ConvertToCamelCase(this string value, string? additionalValue = null)
    {
        var textinfo = new CultureInfo("en-US", false).TextInfo;
        var formattedTitle = "";

        // Remove special characters
        value = Regex.Replace(value, @"[^a-zA-Z0-9\s]", "");

        if (value.Contains(' '))
        {
            var splitValues = value.Split(" ").ToList();

            for (var i = 0; i < splitValues.Count; i++)
            {
                var splitValue = splitValues[i];
                if (i == 0)
                    formattedTitle += splitValue.ToLower();
                else
                    formattedTitle += textinfo.ToTitleCase(splitValue.ToLower());
            }
        }
        else
        {
            formattedTitle = value.ToLower();
        }

        if (additionalValue != null)
        {
            // Remove special characters from additionalValue as well
            additionalValue = Regex.Replace(additionalValue, @"[^a-zA-Z0-9]", "");
            formattedTitle += additionalValue;
        }

        return formattedTitle;
    }
    
    public static string FormatImageUrl(this string imageUrl, int imageWidth, string tenantId)
    {
        var formattedImageUrl = imageUrl
            .Replace("tr=w-2000", $"w={imageWidth}")
            .Replace("tr=w-1800", $"w={imageWidth}")
            .Replace("tr=w-1000", $"w={imageWidth}")
            .Replace("tr=w-900", $"w={imageWidth}")
            .Replace("tr=w-800", $"w={imageWidth}")
            .Replace("tr=w-700", $"w={imageWidth}")
            .Replace("tr=w-600", $"w={imageWidth}")
            .Replace("tr=w-500", $"w={imageWidth}")
            .Replace("tr=w-400", $"w={imageWidth}")
            .Replace("w=2000", $"w={imageWidth}")
            .Replace("w=1800", $"w={imageWidth}")
            .Replace("w=1000", $"w={imageWidth}")
            .Replace("w=900", $"w={imageWidth}")
            .Replace("w=800", $"w={imageWidth}")
            .Replace("w=700", $"w={imageWidth}")
            .Replace("w=600", $"w={imageWidth}")
            .Replace("w=500", $"w={imageWidth}")
            .Replace("w=400", $"w={imageWidth}")
            .Replace($"https://ik.imagekit.io/rqlzhe7ko/{tenantId}/trips/", $"https://travaloud.azureedge.net/{tenantId}/assets/images/")
            .Replace($"https://ik.imagekit.io/rqlzhe7ko/{tenantId}/", $"https://travaloud.azureedge.net/{tenantId}/assets/images/").
            Replace($"https://travaloud.azureedge.net/{tenantId}/assets/images/", $"https://travaloudcdn.azureedge.net/{tenantId}/assets/images/");

        if (!formattedImageUrl.Contains("?w="))
            formattedImageUrl += $"?w={imageWidth}";
        
        return formattedImageUrl;
    }
}