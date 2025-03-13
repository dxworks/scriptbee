using System.Text.Json;
using DxWorks.ScriptBee.Plugin.Api.Model;
using ScriptBee.Domain.Model.Project;
using ScriptBee.UseCases.Project.ProjectStructure;

namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

public record WebCreateScriptCommand(
    string Path,
    string Language,
    IEnumerable<WebScriptParameter>? Parameters
)
{
    public CreateScriptCommand Map(ProjectId projectId)
    {
        return new CreateScriptCommand(
            projectId,
            Path,
            Language,
            (Parameters ?? []).Select(p => new ScriptParameter
            {
                Name = p.Name,
                Type = p.Type,
                Value = GetValue(p.Type, p.Value),
            })
        );
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
