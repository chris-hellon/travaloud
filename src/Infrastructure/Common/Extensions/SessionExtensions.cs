using System.Text.Json;

namespace Travaloud.Infrastructure.Common.Extensions;

public static class SessionExtensions
{
    public static T GetOrCreateObjectFromSession<T>(this ISession? session, string key) where T : new()
    {
        if (session == null) return default!;
        
        var jsonString = session.GetString(key);

        if (jsonString == null)
        {
            var newObject = new T();
            jsonString = JsonSerializer.Serialize(newObject);
            session.SetString(key, jsonString);
            return newObject;
        }

        var existingObject = JsonSerializer.Deserialize<T>(jsonString);
        return (existingObject ?? default(T))!;

    }

    public static void UpdateObjectInSession<T>(this ISession? session, string key, T updatedObject)
    {
        var jsonString = JsonSerializer.Serialize(updatedObject);
        session?.SetString(key, jsonString);
    }
}