using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScriptBee.Rest.Converters;

public sealed class JsonStringEnumMemberConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        typeToConvert = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;
        return typeToConvert.IsEnum;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var enumType = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;

        var converterType = typeof(EnumMemberConverter<>).MakeGenericType(enumType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    private sealed class EnumMemberConverter<TEnum> : JsonConverter<TEnum>
        where TEnum : struct, Enum
    {
        private static readonly Dictionary<string, TEnum> ReadMap;
        private static readonly Dictionary<TEnum, string> WriteMap;

        static EnumMemberConverter()
        {
            ReadMap = new(StringComparer.Ordinal);
            WriteMap = new();

            foreach (
                var field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static)
            )
            {
                var value = (TEnum)field.GetValue(null)!;

                var enumMember = field.GetCustomAttribute<EnumMemberAttribute>();

                var text = enumMember?.Value ?? field.Name;

                ReadMap[text] = value;
                WriteMap[value] = text;
            }
        }

        public override TEnum Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            var value = reader.GetString();

            if (value is not null && ReadMap.TryGetValue(value, out var result))
            {
                return result;
            }

            throw new JsonException($"Unable to convert '{value}' to {typeof(TEnum).Name}.");
        }

        public override void Write(
            Utf8JsonWriter writer,
            TEnum value,
            JsonSerializerOptions options
        )
        {
            if (WriteMap.TryGetValue(value, out var text))
            {
                writer.WriteStringValue(text);
                return;
            }

            writer.WriteStringValue(value.ToString());
        }
    }
}
