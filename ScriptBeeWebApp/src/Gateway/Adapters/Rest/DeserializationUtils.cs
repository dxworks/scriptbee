using System.Text.Json;

namespace ScriptBee.Rest;

public class DeserializationUtils
{
    public static T ConvertTo<T>(object value)
    {
        if (value is JsonElement element)
            return element.Deserialize<T>()!;

        return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value))!;
    }
}
