using System.Collections;
using System.Reflection;

namespace Travaloud.Application.Common.Extensions;

public static class ObjectExtensions
{
    public static IDictionary<string, string?> FlattenObject(this object? obj)
    {
        var dictionary = new Dictionary<string, string?>();
        FlattenObject(obj, dictionary);
        return dictionary;
    }

    private static void FlattenObject(object? obj, IDictionary<string, string?> dictionary, string prefix = "")
    {
        if (obj == null)
            return;

        foreach (var property in obj.GetType().GetProperties())
        {
            if (property.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                continue; // Skip properties with JsonIgnore attribute

            var jsonProperty = property.GetCustomAttribute<JsonPropertyAttribute>();
            var propertyName = jsonProperty?.PropertyName ?? property.Name;
            var key = string.IsNullOrEmpty(prefix) ? propertyName : $"{prefix}[{propertyName}]";
            var value = property.GetValue(obj);

            if (value == null)
                continue;

            if (IsSimpleType(property.PropertyType))
            {
                dictionary[key] = value.ToString();
            }
            else if (value is IEnumerable enumerable and not string)
            {
                var index = 0;
                foreach (var item in enumerable)
                {
                    FlattenObject(item, dictionary, $"{key}[{index++}]");
                }
            }
            else
            {
                FlattenObject(value, dictionary, key);
            }
        }
    }

    private static bool IsSimpleType(Type type)
    {
        return
            type.IsValueType ||
            type.IsPrimitive ||
            new[]
            {
                typeof(string), typeof(decimal), typeof(DateTime), typeof(DateTimeOffset),
                typeof(TimeSpan), typeof(Guid)
            }.Contains(type) ||
            Convert.GetTypeCode(type) != TypeCode.Object;
    }
}