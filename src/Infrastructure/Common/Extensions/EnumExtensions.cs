using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Travaloud.Infrastructure.Common.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum enumValue)
    {
        var attr = enumValue.GetType().GetField(enumValue.ToString())!
            .GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attr.Length > 0)
            return ((DescriptionAttribute)attr[0]).Description;
        var result = enumValue.ToString();
        result = Regex.Replace(result, "([a-z])([A-Z])", "$1 $2");
        result = Regex.Replace(result, "([A-Za-z])([0-9])", "$1 $2");
        result = Regex.Replace(result, "([0-9])([A-Za-z])", "$1 $2");
        result = Regex.Replace(result, "(?<!^)(?<! )([A-Z][a-z])", " $1");
        return result;
    }

    public static List<string> GetDescriptionList(this Enum enumValue)
    {
        var result = enumValue.GetDescription();
        return result.Split(',').ToList();
    }
    
    public static string ToDescriptionString(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        if (field is null)
        {
            return value.ToString().ToLower();
        }

        return Attribute.GetCustomAttributes(field, typeof(DescriptionAttribute), false) is DescriptionAttribute[] { Length: > 0 } attributes
            ? attributes[0].Description
            : value.ToString().ToLower();
    }
}