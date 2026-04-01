using System.Text.Json;
using DxWorks.ScriptBee.Plugin.Api.Model;

namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

public record WebScriptParameter(string Name, string Type, object? Value)
{
    public static WebScriptParameter Map(ScriptParameter parameter)
    {
        return new WebScriptParameter(parameter.Name, parameter.Type, parameter.Value);
    }

    public ScriptParameter Map()
    {
        return new ScriptParameter
        {
            Name = Name,
            Type = Type,
            Value = GetValue(Type, Value),
        };
    }

    private static object? GetValue(string type, object? value)
    {
        if (value is null)
        {
            return null;
        }

        var jsonElement = value as JsonElement? ?? default;

        return type switch
        {
            ScriptParameter.TypeString when jsonElement.ValueKind == JsonValueKind.String =>
                jsonElement.GetString(),
            ScriptParameter.TypeBoolean
                when jsonElement.ValueKind is JsonValueKind.True or JsonValueKind.False =>
                jsonElement.GetBoolean(),
            ScriptParameter.TypeInteger when jsonElement.ValueKind == JsonValueKind.Number =>
                jsonElement.GetInt32(),
            ScriptParameter.TypeFloat when jsonElement.ValueKind == JsonValueKind.Number =>
                jsonElement.GetDouble(),
            _ => null,
        };
    }
}
