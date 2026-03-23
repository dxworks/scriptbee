using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Web.EndpointDefinitions.ProjectStructure.Contracts;

public class WebScriptData
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Path { get; init; }
    public required string AbsolutePath { get; init; }
    public required WebScriptLanguage ScriptLanguage { get; init; }
    public required IEnumerable<WebScriptParameter> Parameters { get; init; }

    public static WebScriptData Map(Script script)
    {
        return new WebScriptData
        {
            Id = script.Id.ToString(),
            Name = script.Name,
            Path = script.FilePath,
            AbsolutePath = script.AbsoluteFilePath,
            ScriptLanguage = WebScriptLanguage.Map(script.ScriptLanguage),
            Parameters = script.Parameters.Select(WebScriptParameter.Map),
        };
    }
}
